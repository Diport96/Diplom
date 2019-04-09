#ifndef MqttClient_h
#define MqttClient_h

#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"

#define REQUEST_TO_CONNECT "CONNECTION"
#define PERMIT_TO_CONNECT "CONNACK"
#define REQUEST_TO_DISCONNECT "DISCONNECT"
#endif

class MqttClientSensor {
private:    
    const char* ID;
	const char* Name;
	const char* Type;
	double Value;    
    bool connected;
    PubSubClient* _client;    

    //Json параметры
    const int capacity = JSON_OBJECT_SIZE(12);
    StaticJsonDocument<capacity> sensorDoc; // Данные об подключаемом усторйстве
    StaticJsonDocument<capacity> resultDoc;


public:     
    MqttClientSensor(const char*, const char*, Client& client, double&, const char*, IPAddress, uint16_t); 
        

    bool Connect(); //Not implemented
    void Run(); //Not implemented
};



