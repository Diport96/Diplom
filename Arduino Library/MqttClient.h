#ifndef MqttClient_h
#define MqttClient_h

#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v5.13.4.h"

#define REQUEST_TO_CONNECT "CONNECT"
#define PERMIT_TO_CONNECT "CONNACK"
#define REQUEST_TO_DISCONNECT "DISCONNECT"

#define TOPIC_FOR_CONNECTION = "Connection"
#endif

class MqttClientSensor {
private:    
    const char* id;
	const char* name;
	const char* type;
	double value;    
    bool connected;
    const char* topic;
    PubSubClient* _client;    

    //Json параметры
    const int capacity = JSON_OBJECT_SIZE(12);
    StaticJsonBuffer<capacity> jb;
    JsonObject& sensorObj = jb.createObject();
	JsonObject& resultObj = jb.createObject();

    //Методы
    void callback(char* topic, byte* payload, unsigned int length);
    bool distributeValues(); // Not implemented

public:     
    MqttClientSensor(const char*, const char*, Client& client, double&, const char*, IPAddress, uint16_t, const char*); 
    bool Connect();   
};



