
#include "Default.h"

#include "./USB/usb.h"
#include "HardwareProfile.h"
#include "./USB/usb_function_hid.h"

#include "MainStation.h"
#include "EP.h"
#include "USB.h"
#include "UART.h"

/** VARIABLES ******************************************************/
#pragma udata

#if defined(__18F14K50) || defined(__18F13K50) || defined(__18LF14K50) || defined(__18LF13K50) 
    #pragma udata usbram2
#elif defined(__18F2455) || defined(__18F2550) || defined(__18F4455) || defined(__18F4550)\
    || defined(__18F2458) || defined(__18F2453) || defined(__18F4558) || defined(__18F4553)
    #pragma udata USB_VARIABLES=0x500
#elif defined(__18F4450) || defined(__18F2450)
    #pragma udata USB_VARIABLES=0x480
#else
    #pragma udata
#endif

extern unsigned char ReceivedDataBuffer[64];
extern unsigned char ToSendDataBuffer[64];
#pragma udata

extern USB_HANDLE USBOutHandle;
extern USB_HANDLE USBInHandle;

extern int8 OperationEnabled;

extern int16 DeviceID;

void USBInit()
{
    //initialize the variable holding the handle for the last transmission
    USBOutHandle = 0;
    USBInHandle = 0;
}

void USBInitEndPoint()
{
    //enable the HID endpoint
    USBEnableEndpoint(HID_EP,USB_IN_ENABLED|USB_OUT_ENABLED|USB_HANDSHAKE_ENABLED|USB_DISALLOW_SETUP);
    //Re-arm the OUT endpoint for the next packet
    USBOutHandle = HIDRxPacket(HID_EP,(BYTE*)&ReceivedDataBuffer,64);
}

void USBWrite()
{
	USBInHandle = HIDTxPacket(HID_EP,(BYTE*)&ToSendDataBuffer[0],64);
}

int8 USBBusy()
{
	return HIDTxHandleBusy(USBInHandle);
}

void USBUpdate()
{
	int a,b;
    // User Application USB tasks
    if((USBDeviceState < CONFIGURED_STATE)||(USBSuspendControl==1)) return;
    
    if(!HIDRxHandleBusy(USBOutHandle))				//Check if data was received from the host.
    {   
        switch(ReceivedDataBuffer[0])				//Look at the data the host sent, to see what kind of application specific command it sent.
        {
			case 0x01://GET Device Type
				if(USBBusy()==0)
                {
					ToSendDataBuffer[0]=0x01;
					ToSendDataBuffer[1]=EPGetType();
					USBWrite();
                }
				break;
			case 0x02:
				if(USBBusy()==0)
				{
					ToSendDataBuffer[0]=0x02;
					ToSendDataBuffer[1]=SettingsReadInt8(0);
					ToSendDataBuffer[2]=SettingsReadInt8(1);
					USBWrite();
				}
				break;
			case 0x03:
				if(USBBusy()==0)
				{
					ToSendDataBuffer[0]=0x03;
					SettingsWriteInt8(0,ReceivedDataBuffer[1]);
					SettingsWriteInt8(1,ReceivedDataBuffer[2]);
					DeviceID=SettingsReadInt16(0);
					USBWrite();
				}
				break;
        }

        if(!HIDTxHandleBusy(USBInHandle))
        {
            USBInHandle = HIDTxPacket(HID_EP,(BYTE*)&ToSendDataBuffer[0],64);
        }
		ReceivedDataBuffer[0]=0;
        //Re-arm the OUT endpoint for the next packet
        USBOutHandle = HIDRxPacket(HID_EP,(BYTE*)&ReceivedDataBuffer,64);
    }
}
