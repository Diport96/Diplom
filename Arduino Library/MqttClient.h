

#ifndef MqttClient_h
#define MqttClient_h

#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"




class MqttClient
{
private:
    Client* _client;

public:
    MqttClient(); //Not implemented
    MqttClient(Client& client); //Not implemented
    MqttClient(Client& client, double& sensorValue); //Not implemented
    MqttClient(Client& client, bool& switchValue); //Not implemented
    MqttClient(Client& client, double& sensorValue, const char * sensorType); //Not implemented
    MqttClient(Client& client, bool& switchValue, const char * switchType); //Not implemented
    
    bool Connect(); //Not implemented
    void Run(); //Not implemented
}