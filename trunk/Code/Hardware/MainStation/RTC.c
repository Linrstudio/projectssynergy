#include "RTC.h"
#include "Kismet.h"
#include "Default.h"

int8	RTCSecond;
int8	RTCMinute;
int8	RTCHour;
int16	RTCDay;
int8	RTCEdge;//possibly a better way of reading TMR1

void SetTimer(const int _TimerIndex,int16 _Time,int8 _Event)
{
	int idx;
	_Index<<=2;
	idx=255;
	idx-=_Index;
	Set16(idx,_Timer.Time);
	Set8(idx+2,_Timer.Event);
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

void RTCUpdate()
{
	//RA4=RTCSecond&1;
	int e=(TMR1H>>4)&1;
	if(e==RTCEdge)
	{
		RTCEdge=1-RTCEdge;
		
		RTCSecond++;
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
					KismetExecuteEvent(0,4);
				}
				KismetExecuteEvent(0,3);
			}
			KismetExecuteEvent(0,2);
		}
		KismetExecuteEvent(0,1);
	}
}
