#ifndef SETTINGS_H
#define SETTINGS_H

#include <pic18.h>
#include "Default.h"

extern void SettingsInit();

extern void  SettingsWriteInt8(int8 _Addr,int8 _Data);
extern void  SettingsWriteInt16(int8 _Addr,int16 _Data);
extern int8  SettingsReadInt8(int8 _Addr);
extern int16 SettingsReadInt16(int8 _Addr);

#endif