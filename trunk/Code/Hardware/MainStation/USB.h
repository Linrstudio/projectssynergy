#ifndef USB_H
#define USB_H


#include "Default.h"



void USBInit();
void USBInitEndPoint();
void USBWrite();
int8 USBBusy();//determines if one could read/write or not
void USBUpdate();

#endif
