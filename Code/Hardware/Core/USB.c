#include "Default.h"

#include "USB.h"

#include "usb_config.h"
#include "./USB/usb_device.h"
#include "./USB/usb.h"
#include "./USB/usb_function_cdc.h"

#include "usb_config.h"
#include "USB\usb_device.h"
#include "USB\usb.h"

#define USBBUFFERSIZE 64

int8 USBOutBuffer[USBBUFFERSIZE];
int8 USBOutIndex=0;

int8 USBInBuffer[USBBUFFERSIZE];
int8 USBInIndex=0;
int8 USBInSize=0;

int8 USBPollDelay=0;

void USBInit()
{
	unsigned char i;
	USBDeviceAttach();
}
void USBUpdate()
{
	//if(USBPollDelay&32)
	{
		USBDeviceTasks();
		CDCTxService();
	}
	USBPollDelay++;
}

int8 USBCanWrite()
{
	if((USBDeviceState < CONFIGURED_STATE)||(USBSuspendControl==1))
		return 0;
	return 255;
}

int8 USBForceWrite()
{
	if(!mUSBUSARTIsTxTrfReady())
	{
		return 0;
	}
	if(USBOutIndex!=0)
	{
		putUSBUSART(USBOutBuffer, USBOutIndex);
		CDCTxService();
		USBOutIndex=0;
	}
	return 255;
}
void USBWriteInt8(const int8 _Data)
{
	USBOutBuffer[USBOutIndex]=_Data;
	USBOutIndex++;
	if(USBOutIndex==USBBUFFERSIZE)
	{
		USBForceWrite();
	}
}

int8 USBCanRead()
{
	if(USBInIndex>=USBInSize||USBInIndex>=USBBUFFERSIZE)
	{
		USBInSize = getsUSBUSART(&USBInBuffer[0],USBBUFFERSIZE);
		USBInIndex=0;
		if(USBInSize==0)return 0;
	}
	return 255;
}

int8 USBReadInt8()
{
	int8 d;
	if(!USBCanRead())return 0;
	d = USBInBuffer[USBInIndex];
	USBInIndex++;
	return d;
}

int16 USBReadInt16()
{
	union
	{
		int16 ret;
		struct
		{
			int8 dat1;
			int8 dat2;
		}st;
	}un;

	un.st.dat1=USBReadInt8();
	un.st.dat2=USBReadInt8();

	return un.ret;
}
