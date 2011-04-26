// #define DIGITAL_IN8
// #define DIGITAL_OUT8
#define RELAY8

#include "Default.h"

void SettingsInit();
void SettingsWriteInt8(int8 _Addr,int8 _Data);
int8 SettingsReadInt8(int8 _Addr);
int16 SettingsReadInt16(int8 _Addr);
