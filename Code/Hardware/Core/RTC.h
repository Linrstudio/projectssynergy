#ifndef RTC_H
#define RTC_H
#include "Default.h"

int8 RTCSecond;
int8 RTCMinute;
int8 RTCHour;
int8 RTCDay;

int8 RTCEdge;//possibly a better way of reading TMR1

extern void RTCInit();
extern void RTCUpdate();
#endif