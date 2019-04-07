#pragma once
#include <string>;

static class MqttTopics
{
public:
	static const std::string CONNECTION;
	static const std::string DEVICES;
	static const std::string SWITCHES;
	static const std::string SENSORS;
	static const std::string TERMOMETERS;
};

const std::string MqttTopics::CONNECTION = "Connection";
const std::string MqttTopics::DEVICES = "Devices";
const std::string MqttTopics::SWITCHES = "Devices/Switches";
const std::string MqttTopics::SENSORS = "Devices/Sensors";
const std::string MqttTopics::TERMOMETERS = "Devices/Sensors/Termometers";