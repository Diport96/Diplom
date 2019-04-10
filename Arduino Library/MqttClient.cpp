#include "MqttClient.h"
#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v5.13.4.h"


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
    if(!_client->connect(this->ID)) return false;
    _client.subscribe(TOPIC_FOR_CONNECTION);

    // Формироваие JSON документа
    sensorObj["ID"] = this->id;
    sensorObj["Name"] = this->name;
	sensorObj["Value"] = this->value;
    resultObj["Message_Type"] = REQUEST_TO_CONNECT;
	resultObj["Type"] = this->type;
	resultObj["Class"] = sensorDoc;

    // Конвертирование в строку
    const char* res;
    resultObj.prettyPrintTo(res);

    // Отправка сообщения на сервер
    _client.publish(topic, res);
}

void MqttClientSensor::callback(char* topic, byte* payload, unsigned int length)
{
    if(topic == TOPIC_FOR_CONNECTION)
    {
        char[length] message;
        for(int i = 0; i < length; i++)
        {
            message[i] = (char)payload[i];
        }

        if(message == PERMIT_TO_CONNECT)
        {
            //!!! namespace?
            MqttClientSensor::connected = true;
        }
    }
}

bool MqttClientSensor::distributeValues()
{
    //
}