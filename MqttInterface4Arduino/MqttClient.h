#pragma once
//Singleton& instance = Singleton::Instance();
class MqttClient
{
private:
	MqttClient();

	MqttClient(const MqttClient&) = delete;
	MqttClient& operator=(MqttClient&) = delete;
public:
	static MqttClient& getInstance();
	void Run();
};

