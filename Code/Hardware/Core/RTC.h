#include <pic18.h>
#include "Default.h"

int8 RTCSecond;
int8 RTCMinute;
int8 RTCHour;
int8 RTCDay;

void RTCInit()
{
	TMR1ON=1;//enable timer
	TMR1CS=1;//select external osc ( pin 8&9)
	T1SYNC=1;//dont sync with main osc, i like this one better
	T1OSCEN=1;//enable oscillator
	T1RD16=0;//8bit
	T1RUN=0;//not sure why..

	T1CKPS0=1;//prescale
	T1CKPS1=1;

	TMR1L=0;
	TMR1H=0;
}

void HandleRTC()
{
	//RC0=RTCSecond&1;
	if(TMR1H&(256-16))
	{
		RTCSecond+=TMR1H>>4;
		TMR1H&=15;//trash bits we just read
		if(RTCSecond>=60)
		{
			RTCSecond-=60;
			RTCMinute++;
		
			if(RTCMinute>=60)
			{
				RTCMinute=0;
				RTCHour++;
				
				if(RTCHour>=24)
				{
					RTCHour=0;
					RTCDay++;
				
					if(RTCDay>=7)
					{
						RTCDay=0;
						RC2=1;
					}
				}
			}
		}
	}
}