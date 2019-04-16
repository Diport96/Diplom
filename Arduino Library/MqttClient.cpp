#include "MqttClient.h"
#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"
#include "Time.h"

// MqttClientSensor
MqttClientSensor::MqttClientSensor(const char* id, const char* name, Client& client, double& sensorVal, const char* sensorType, IPAddress ip, uint16_t port)
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
    if(!_client->connect(this->id)) return false;
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

void MqttClientSensor::callback(char* topic, byte* payload, unsigned int length)
{    
    if(topic == TOPIC_FOR_CONNECTION)
    {
        char message[length];
        for(int i = 0; i < length; i++)
        {
            message[i] = (char)payload[i];
        }

        DynamicJsonDocument doc(CAPACITY);
        deserializeJson(doc, message);
        
        if(doc["Message_Type"] == PERMIT_TO_CONNECT)
        {
            if(doc["ID"] == MqttClientSensor::id)
            {   
                connected = true;
                _client->subscribe(TOPIC_FOR_SENSORS);
            }
        }
    }
}

bool MqttClientSensor::PublishValue()
{
    DynamicJsonDocument doc(CAPACITY);
    doc["Message_Type"] = DISTRIBUTION_OF_VALUES;
    doc["ID"] = MqttClientSensor::id;
    doc["Date"] = now();
    doc["Value"] = MqttClientSensor::value;

    char res[128]; // !!! Оптимизировать
    serializeJson(doc, res);
    _client->publish(TOPIC_FOR_SENSORS, res);
}


// MqttClientSwitch
MqttClientSwitch::MqttClientSwitch(const char* id, const char* name, Client& client, bool& switchState, const char* switchType, IPAddress ip, uint16_t port)
{    
    this->id = id;
    this->name = name;
    this->state = switchState;  
    this->type = switchType;
    this->connected = false;    
    this->_client = new PubSubClient(ip, port, client);      
}


bool MqttClientSwitch::Connect()
{
    // Попытка подключения
    if(!_client->connect(this->id)) return false;
    _client->subscribe(TOPIC_FOR_CONNECTION);
    _client->subscribe(TOPIC_FOR_SWITCHES);

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

void MqttClientSwitch::callback(char* topic, byte* payload, unsigned int length)
{    
    if(topic == TOPIC_FOR_CONNECTION)
    {
        char message[length];
        for(int i = 0; i < length; i++)
        {
            message[i] = (char)payload[i];
        }

        DynamicJsonDocument doc(CAPACITY);
        deserializeJson(doc, message);
        
        if(doc["Message_Type"] == PERMIT_TO_CONNECT)
        {
            if(doc["ID"] == MqttClientSwitch::id)
            {   
               connected = true;
               _client->subscribe(TOPIC_FOR_SWITCHES);
            }
        }
    }    
}

bool MqttClientSwitch::PublishValue()
{
    DynamicJsonDocument doc(CAPACITY);
    doc["Message_Type"] = DISTRIBUTION_OF_VALUES;
    doc["ID"] = MqttClientSwitch::id;
    doc["Date"] = now();
    doc["Value"] = MqttClientSwitch::state;

    char res[128]; // !!! Оптимизировать
    serializeJson(doc, res);
    _client->publish(TOPIC_FOR_SWITCHES, res);
}