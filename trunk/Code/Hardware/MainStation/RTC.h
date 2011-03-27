#ifndef RTC_H
#define RTC_H

#include "Default.h"

void SetTimer(int8 _TimerIndex,int8 _Event,int16 _Time);

void RTCInit();
void RTCUpdate();

#endif