#ifndef EP_H
#define EP_H

#include "Default.h"

void EPInit();
void EPUpdate();
int8 EPSend(int16 _DeviceID);
int8 EPPoll(int16 _DeviceID);

#endif