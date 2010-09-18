#ifndef USB_H
#define USB_H


#include "Default.h"

#include "GenericTypeDefs.h"
#include "Compiler.h"
#include "usb_config.h"
#include "./USB/usb_device.h"
#include "./USB/usb.h"

#include "HardwareProfile.h"

#include "./USB/usb_function_hid.h"

void USBInit();
void USBInitEndPoint();

void USBUpdate();

#endif
