#ifndef USB_H
#define USB_H

#include "Default.h"

void USBInit();
void USBUpdate();

int8 USBCanWrite();
void USBWriteInt8(const int8 _Data);
int8 USBForceWrite();

int8 USBCanRead();
int8 USBReadInt8();
int16 USBReadInt16();

#endif
