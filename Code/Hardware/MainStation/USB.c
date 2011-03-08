
#include "Default.h"

#include "./USB/usb.h"
#include "HardwareProfile.h"
#include "./USB/usb_function_hid.h"

#include "MainStation.h"
#include "EP.h"
#include "USB.h"
#include "UART.h"
#include "EEPROM.h"
#include "Kismet.h"

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

extern int8 RTCSecond;
extern int8 RTCMinute;
extern int8 RTCHour;
extern int16 RTCDay;

extern int16 EPBufferSize;

extern int8 OperationEnabled;

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
			case 0x01:
				a=EPPoll(*(int16*)&ReceivedDataBuffer[1]);
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x03;
					if(a!=0)
						ToSendDataBuffer[1]=0;
					else
						ToSendDataBuffer[1]=255;
					USBWrite();
                }
				break;
			case 0x02:
				EPBufferSize=ReceivedDataBuffer[3];
				for(a=0;a<EPBufferSize;a++)Set8(a,ReceivedDataBuffer[a+4]);
				EPSend(*(int16*)&ReceivedDataBuffer[1]);
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x02;
                    USBWrite();
                }
				break;
			case 0x06:
				ToSendDataBuffer[1]=KismetExecuteEvent(((int16*)&ReceivedDataBuffer[1])[0],ReceivedDataBuffer[3]);
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x06;
                    USBWrite();
                }
				break;
			case 0x07:
				OperationEnabled=0;
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x07;
                    USBWrite();
                }
				break;
			case 0x08:
				OperationEnabled=0xff;
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x08;
                    USBWrite();
                }
				break;
			case 0x03://EEPROM Write
				MemoryBeginWrite(((int16*)&ReceivedDataBuffer[1])[0]);
				MemoryWrite(ReceivedDataBuffer[3]);
				MemoryEndWrite();
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x03;
                    USBWrite();
                }
				break;
			case 0x04://EEPROM WRITE PAGE
				MemoryBeginWrite(((int16*)&ReceivedDataBuffer[1])[0]);
				for(a=0;a<32;a++)
					MemoryWrite(ReceivedDataBuffer[3+a]);
				MemoryEndWrite();
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x04;
                    USBWrite();
                }
				break;
			case 0x05://EEPROM READ
				if(!USBBusy())
                {
					MemoryBeginRead(((int16*)&ReceivedDataBuffer[1])[0]);
					ToSendDataBuffer[0]=0x04;
					ToSendDataBuffer[1]=MemoryReadInt8();
					MemoryEndRead();
                    USBWrite();
                }
				break;
			case 0x40://READ TIME
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x40;
					ToSendDataBuffer[1]=RTCSecond;
					ToSendDataBuffer[2]=RTCMinute;
					ToSendDataBuffer[3]=RTCHour;
					INT16(ToSendDataBuffer[4])=RTCDay;
					USBWrite();
                }
				break;
			case 0x41://WRITE TIME
				RTCSecond	=ReceivedDataBuffer[1];
				RTCMinute	=ReceivedDataBuffer[2];
				RTCHour		=ReceivedDataBuffer[3];
				RTCDay		=INT16(ReceivedDataBuffer[4]);
				if(!USBBusy())
                {
					ToSendDataBuffer[0]=0x41;
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
