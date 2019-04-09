#include "MqttClient.h"
#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"


MqttClientSensor::MqttClientSensor( const char* id, const char* name, Client& client, double& sensorVal, const char* sensorType, IPAddress ip, uint16_t port)
{    
    this->ID = id;
    this->Name = name;
    this->Value = sensorVal;  
    this->Type = sensorType;
    this->connected = false;
    this->_client = new PubSubClient(ip, port, client);      
}

bool MqttClientSensor::Connect()
{
    // Попытка подключения
    if(!_client->connect(this->ID)) return false;

    // Формироваие JSON документа
    sensorDoc["ID"] = this->ID;
    sensorDoc["Name"] = this->Name;
	sensorDoc["Value"] = this->Value;
    resultDoc["Message_Type"] = REQUEST_TO_CONNECT;
	resultDoc["Type"] = this->Type;
	resultDoc["Class"] = sensorDoc;

    // !!!
}