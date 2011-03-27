#include "RTC.h"
#include "Kismet.h"
#include "EEPROM.h"
#include "Default.h"

int8	RTCSecond;
int8	RTCMinute;
int8	RTCHour;
int16	RTCDay;
int8	RTCEdge;//possibly a better way of reading TMR1

extern int8 OperationEnabled;

void SetTimer(int8 _TimerIndex,int8 _Event,int16 _Time)
{
	int idx;
	_TimerIndex<<=2;
	idx=251;// minus four so timer 0 becomes 251
	idx-=_TimerIndex;
	//_Time=3;
	RTCMinute=(int8)_Time;
	Set16(idx,_Time);
	Set8(idx+2,_Event);
}

void RTCInit()
{
	T1CONbits.TMR1ON=1;//enable timer
	T1CONbits.TMR1CS=1;//select external osc ( pin 8&9)
	T1CONbits.T1SYNC=1;//dont sync with main osc, i like this one better
	T1CONbits.T1OSCEN=1;//enable oscillator
	//T1CONbits.T1RD16=0;//8bit
	T1CONbits.T1RUN=0;//not sure why..
	
	T1CONbits.T1CKPS0=1;//prescale
	T1CONbits.T1CKPS1=1;
	
	TMR1L=0;
	TMR1H=0;
	
	RTCEdge=0;
	
	RTCSecond=0;
	RTCMinute=0;
	RTCHour=0;
}

void UpdateTimers()
{
	int8 timercount;
	int8 addr=255;
	int8 a;
	if(OperationEnabled==0)return;

	MemoryBeginRead(0);
	timercount = MemoryReadInt8();
	MemoryEndRead();

	addr-=timercount<<2;
	while(timercount)
	{
		a=Get16(addr);
		if(a==1)KismetExecuteEvent(0,Get8(addr+2));
		if(a!=0)Set16(addr,a-1);

		addr+=4;
		timercount--;
	}
}

void RTCUpdate()
{
	//RA4=RTCSecond&1;
	int e=(TMR1H>>4)&1;
	if(e==RTCEdge)
	{
		RTCEdge=1-RTCEdge;
		
		RTCSecond++;
		
		UpdateTimers();
		
		if(RTCSecond>=60)
		{
			RTCSecond=0;
			RTCMinute++;
			
			if(RTCMinute>=60)
			{
				RTCMinute=0;
				RTCHour++;
				
				if(RTCHour>=24)
				{
					RTCHour=0;
					RTCDay++;
					KismetExecuteEvent(0,5);
				}
				KismetExecuteEvent(0,4);
			}
			KismetExecuteEvent(0,3);
		}
		KismetExecuteEvent(0,2);
	}
}
