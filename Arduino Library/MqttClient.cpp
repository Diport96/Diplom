#include "MqttClient.h"
#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"
#include "Time.h"

// MqttClientSensor
MqttClientSensor::MqttClientSensor(const char *id, const char *name, Client &client, double &sensorVal, const char *sensorType, IPAddress ip, uint16_t port)
{
    this->id = id;
    this->name = name;
    this->value = sensorVal;
    this->type = sensorType;
    this->connected = false;
    this->_client = new PubSubClient(ip, port, client);
}
bool MqttClientSensor::Connect()
{
    // Попытка подключения
    if (!_client->connect(this->id))
        return false;
    _client->subscribe(TOPIC_FOR_CONNECTION);

    // JSON параметры
    StaticJsonDocument<CAPACITY> sensorObj;
    StaticJsonDocument<CAPACITY> resultObj;

    // Формироваие JSON документа
    sensorObj["ID"] = this->id;
    sensorObj["Name"] = this->name;
    sensorObj["Value"] = this->value;

    char sensorObjToJson[64];
    serializeJson(sensorObj, sensorObjToJson);

    resultObj["Message_Type"] = REQUEST_TO_CONNECT;
    resultObj["Type"] = this->type;
    resultObj["Class"] = sensorObjToJson;

    // Конвертирование в строку
    char res[128]; // !!! Оптимизировать
    serializeJson(resultObj, res);

    // Отправка сообщения на сервер
    _client->publish(REQUEST_TO_CONNECT, res);

    return true;
}
void MqttClientSensor::Disconnect()
{
    _client->unsubscribe(TOPIC_FOR_SENSORS);
    _client->unsubscribe(TOPIC_FOR_CONNECTION);    
    _client->disconnect();
    connected = false;
}
void MqttClientSensor::callback(char *topic, byte *payload, unsigned int length)
{
    if (topic == TOPIC_FOR_CONNECTION)
    {
        char message[length];
        for (int i = 0; i < length; i++)
        {
            message[i] = (char)payload[i];
        }

        DynamicJsonDocument doc(CAPACITY);
        deserializeJson(doc, message);

        if (doc["Message_Type"] == PERMIT_TO_CONNECT)
        {
            if (doc["ID"] == MqttClientSensor::id)
            {
                _client->subscribe(TOPIC_FOR_SENSORS);
                connected = true;
            }
        }
    }
}
bool MqttClientSensor::PublishValue()
{
    if (!this->connected)
        return false;
    DynamicJsonDocument doc(CAPACITY);
    doc["Message_Type"] = DISTRIBUTION_OF_VALUES;
    doc["ID"] = MqttClientSensor::id;
    doc["Date"] = now();
    doc["Value"] = MqttClientSensor::value;

    char res[128]; // !!! Оптимизировать
    serializeJson(doc, res);

    char *publishConcat;
    strcat(publishConcat, TOPIC_FOR_CONNECTION);
    strcat(publishConcat, "/");
    strcat(publishConcat, this->id);

    _client->publish(TOPIC_FOR_SENSORS, res);
    _client->publish(publishConcat, res);
    return true;
}

// MqttClientSwitch
MqttClientSwitch::MqttClientSwitch(const char *id, const char *name, Client &client, bool &switchState, const char *switchType, IPAddress ip, uint16_t port)
{
    this->id = id;
    this->name = name;
    this->state = switchState;
    this->type = switchType;
    this->connected = false;
    this->_client = new PubSubClient(ip, port, client);
    this->timestamp = millis();
}
bool MqttClientSwitch::Connect()
{
    // Попытка подключения
    if (!_client->connect(this->id))
        return false;
    _client->subscribe(TOPIC_FOR_CONNECTION);

    // JSON параметры
    StaticJsonDocument<CAPACITY> switchObj;
    StaticJsonDocument<CAPACITY> resultObj;

    // Формироваие JSON документа
    switchObj["ID"] = this->id;
    switchObj["Name"] = this->name;
    switchObj["Value"] = this->state;

    char switchObjToJson[64];
    serializeJson(switchObj, switchObjToJson);

    resultObj["Message_Type"] = REQUEST_TO_CONNECT;
    resultObj["Type"] = this->type;
    resultObj["Class"] = switchObjToJson;

    // Конвертирование в строку
    char res[128]; // !!! Оптимизировать
    serializeJson(resultObj, res);

    // Отправка сообщения на сервер
    _client->publish(REQUEST_TO_CONNECT, res);

    return true;
}
void MqttClientSwitch::Disconnect()
{
    _client->unsubscribe(TOPIC_FOR_SWITCHES);
    _client->unsubscribe(TOPIC_FOR_CONNECTION);
    if (options->GetControl() == SwitchToSignal)
    {
        if (options->GetSensorId())
        {
            char *topicConcat;
            strcat(topicConcat, TOPIC_FOR_CONNECTION);
            strcat(topicConcat, "/");
            strcat(topicConcat, options->GetSensorId());
            _client->unsubscribe(topicConcat);
        }
    }
    running = false;
    _client->disconnect();
    connected = false;
}
void MqttClientSwitch::callback(char *topic, byte *payload, unsigned int length)
{
    if (topic == TOPIC_FOR_CONNECTION)
    {
        char message[length];
        for (int i = 0; i < length; i++)
        {
            message[i] = (char)payload[i];
        }

        DynamicJsonDocument doc(CAPACITY);
        deserializeJson(doc, message);

        if (doc["Message_Type"] == PERMIT_TO_CONNECT)
        {
            if (doc["ID"] == MqttClientSwitch::id)
            {
                SwitchOptions *_options;
                const char *controlType = doc["Control"];
                if (controlType == "No")
                {
                    _options = new SwitchOptions();
                }
                else if (controlType == "SwitchToDelay")
                {
                    int delayVal = doc["Delay"];
                    bool to = doc["ValueTo"];
                    _options = new SwitchOptions(delayVal, to);
                }
                else if (controlType == "SwitchToSignal")
                {
                    const char *_id = doc["ID"];
                    bool to = doc["ValueTo"];
                    _options = new SwitchOptions(_id, to);
                }

                this->SetOptions(_options);
                this->_client->subscribe(TOPIC_FOR_SWITCHES);
                connected = true;
            }
        }
    }
    else if (topic == TOPIC_FOR_SWITCHES)
    {
        char message[length];
        for (int i = 0; i < length; i++)
        {
            message[i] = (char)payload[i];
        }

        DynamicJsonDocument doc(CAPACITY);
        deserializeJson(doc, message);
        if (doc["Message_Type"] == CHANGE_SWITCH_STATE)
        {
            if (doc["ID"] == this->id)
            {
                this->state = doc["Value"];
            }
        }
        else if (doc["Message_Type"] == SET_NEW_SWITCH_OPTIONS)
        {
            if (doc["ID"] == MqttClientSwitch::id)
            {
                SwitchOptions *_options;
                const char *controlType = doc["Control"];
                if (controlType == "No")
                {
                    _options = new SwitchOptions();
                }
                else if (controlType == "SwitchToDelay")
                {
                    int delayVal = doc["Delay"];
                    bool to = doc["ValueTo"];
                    _options = new SwitchOptions(delayVal, to);
                }
                else if (controlType == "SwitchToSignal")
                {
                    const char *_id = doc["ID"];
                    bool to = doc["ValueTo"];
                    _options = new SwitchOptions(_id, to);
                }

                this->SetOptions(_options);
            }
        }
    }
    else
    {
        if (this->options)
        {
            char *topicConcat;
            strcat(topicConcat, TOPIC_FOR_CONNECTION);
            strcat(topicConcat, "/");
            strcat(topicConcat, this->options->GetSensorId());
            if (topic == topicConcat)
            {
                char message[length];
                for (int i = 0; i < length; i++)
                {
                    message[i] = (char)payload[i];
                }

                DynamicJsonDocument doc(CAPACITY);
                deserializeJson(doc, message);

                const char *value = doc["Value"];
                if (value == "1" || value == "true")
                {
                    this->state = this->options->GetValueTo();
                }
            }
        }
    }
}
bool MqttClientSwitch::PublishValue()
{
    if (!this->connected)
        return false;
    DynamicJsonDocument doc(CAPACITY);
    doc["Message_Type"] = DISTRIBUTION_OF_VALUES;
    doc["ID"] = MqttClientSwitch::id;
    doc["Date"] = now();
    doc["Value"] = MqttClientSwitch::state;

    char res[128]; // !!! Оптимизировать
    serializeJson(doc, res);
    _client->publish(TOPIC_FOR_SWITCHES, res);
    return true;
}
void MqttClientSwitch::SetOptions(SwitchOptions *_options)
{
    if (options)
    {
        if (options->GetControl() == SwitchToSignal)
        {
            if (options->GetSensorId())
            {
                char *topicConcat;
                strcat(topicConcat, TOPIC_FOR_CONNECTION);
                strcat(topicConcat, "/");
                strcat(topicConcat, options->GetSensorId());
                _client->unsubscribe(topicConcat);
            }
        }
    }
    options = _options;
}
// Запускать в цикле
void MqttClientSwitch::Run()
{
    if (!this->options || !this->connected)
        return;

    switch (this->options->GetControl())
    {
    case No:
    {
        if (!this->running)
        {
            _client->subscribe(TOPIC_FOR_SWITCHES);
            this->running = true;
        }
        break;
    }
    case SwitchToDelay:
    {
        if (millis() - timestamp > this->options->GetDelay())
        {
            this->state = this->options->GetValueTo();
            timestamp = millis();
        }
        break;
    }
    case SwitchToSignal:
    {
        if (!this->running)
        {
            char *topicConcat;
            strcat(topicConcat, TOPIC_FOR_CONNECTION);
            strcat(topicConcat, "/");
            strcat(topicConcat, this->options->GetSensorId());
            this->_client->subscribe(topicConcat);
            this->running = true;
        }
    }
    default:
        break;
    }
}

// SwitchOptions
SwitchOptions::SwitchOptions()
{
    this->control = No;
}
SwitchOptions::SwitchOptions(int switchDelay, bool _valueTo)
{
    this->delayToSwitch = switchDelay;
    this->valueTo = _valueTo;
    this->control = SwitchToDelay;
}
SwitchOptions::SwitchOptions(const char *id, bool _valueTo)
{
    this->sensorId = id;
    this->valueTo = _valueTo;
    this->control = SwitchToSignal;
}
SwitchControl SwitchOptions::GetControl()
{
    return this->control;
}
int SwitchOptions::GetDelay()
{
    return this->delayToSwitch;
}
bool SwitchOptions::GetValueTo()
{
    return this->valueTo;
}
const char *SwitchOptions::GetSensorId()
{
    return this->sensorId;
}