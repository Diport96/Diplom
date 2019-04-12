#ifndef MqttClient_h
#define MqttClient_h

#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"

#define REQUEST_TO_CONNECT "CONNECT"
#define PERMIT_TO_CONNECT "CONNACK"
#define REQUEST_TO_DISCONNECT "DISCONNECT"
#define DISTRIBUTION_OF_VALUES = "DISTRIBUTE"

#define TOPIC_FOR_CONNECTION = "Connection"
#define TOPIC_FOR_SENSORS = "Devices/Sensors"
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
    StaticJsonDocument<capacity> sensorObj;
    StaticJsonDocument<capacity> resultObj;
    
    //Методы
    void callback(char* topic, byte* payload, unsigned int length);

public:     
    MqttClientSensor(const char*, const char*, Client& client, double&, const char*, IPAddress, uint16_t, const char*); 
    bool Connect();   
    bool PublishValue();    
};



