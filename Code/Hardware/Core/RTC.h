#ifndef RTC_H
#define RTC_H

#include "Default.h"

static int8 RTCSecond;
static int8 RTCMinute;
static int8 RTCHour;
static int8 RTCDay;

static int8 RTCEdge;//possibly a better way of reading TMR1

void RTCInit();
void RTCUpdate();

#endif