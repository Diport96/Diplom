#ifndef MqttClient_h
#define MqttClient_h

#include "PubSubClient.h"
#include "Client.h"
#include "ArduinoJson-v6.10.0.h"

#define REQUEST_TO_CONNECT "CONNECT"
#define PERMIT_TO_CONNECT "CONNACK"
#define REQUEST_TO_DISCONNECT "DISCONNECT"
#define DISTRIBUTION_OF_VALUES "DISTRIBUTE"
#define CHANGE_SWITCH_STATE "STATE"
#define SET_NEW_SWITCH_OPTIONS "SETOPTIONS"

#define TOPIC_FOR_CONNECTION "Connection"
#define TOPIC_FOR_SENSORS "Devices/Sensors"
#define TOPIC_FOR_SWITCHES "Devices/Switches"
#endif

const int CAPACITY = JSON_OBJECT_SIZE(12);
enum SwitchControl
{
  No,
  SwitchToDelay,
  SwitchToSignal
};

class MqttClientSensor
{
private:
  const char *id;
  const char *name;
  const char *type;
  double value;
  bool connected;
  PubSubClient *_client;

  void callback(char *topic, byte *payload, unsigned int length);

public:
  MqttClientSensor(const char *, const char *, Client &client, double &, const char *, IPAddress, uint16_t);
  bool Connect();
  void Disconnect();
  bool PublishValue();
};

struct SwitchOptions
{
private:
  const char *sensorId;  // Мдентификатор переключателя
  int delayToSwitch;     // Задержка
  SwitchControl control; // Каким образом осуществляется переключение
  bool valueTo;          // На какое значение переключать: true/fale
public:
  SwitchOptions();
  SwitchOptions(int, bool);
  SwitchOptions(const char *, bool);

  SwitchControl GetControl();
  int GetDelay();
  bool GetValueTo();
  const char *GetSensorId();
};

class MqttClientSwitch
{
private:
  const char *id;
  const char *name;
  const char *type;
  bool state;
  bool running;
  bool connected;
  long unsigned int timestamp;
  PubSubClient *_client;
  SwitchOptions *options;

  void callback(char *topic, byte *payload, unsigned int length);
  void Run();

public:
  MqttClientSwitch(const char *, const char *, Client &client, bool &, const char *, IPAddress, uint16_t);
  bool Connect();
  void Disconnect();
  bool PublishValue();
  void SetOptions(SwitchOptions *);
};