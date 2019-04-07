#include "stdafx.h"
#include "MqttClient.h"


MqttClient::MqttClient()
{
}

MqttClient & MqttClient::getInstance()
{
	static MqttClient  instance;
	return instance;
}
