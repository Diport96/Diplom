#include "MqttClient.h"
#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"
#include "Time.h"


MqttClientSensor::MqttClientSensor(const char* id, const char* name, Client& client, double& sensorVal, const char* sensorType, IPAddress ip, uint16_t port, const char* mqttTopic)
{    
    this->id = id;
    this->name = name;
    this->value = sensorVal;  
    this->type = sensorType;
    this->connected = false;
    this->topic = mqttTopic;
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
    _client->publish(topic, res);
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
            {   //!!!
                MqttClientSensor::connected = true;
                MqttClientSensor::_client->subscribe(TOPIC_FOR_SENSORS);
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

