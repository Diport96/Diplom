#pragma once
#include <string>

static class MqttMessageTypes
{
public:
	static const std::string REQUSET_TO_CONNECT;
	static const std::string REQUSET_TO_DISCONNECT;
	static const std::string BROADCAST_CONNECTION;
	static const std::string PERMIT_TO_CONNECT;
	static const std::string DISTRIBUTION_OF_VALUES;
};


const std::string MqttMessageTypes::REQUSET_TO_CONNECT = "CONNECT";
const std::string MqttMessageTypes::REQUSET_TO_DISCONNECT = "DISCONNECT";
const std::string MqttMessageTypes::BROADCAST_CONNECTION = "HELLO";
const std::string MqttMessageTypes::PERMIT_TO_CONNECT = "CONNACK";
const std::string MqttMessageTypes::DISTRIBUTION_OF_VALUES = "DISTRIBUTE";
