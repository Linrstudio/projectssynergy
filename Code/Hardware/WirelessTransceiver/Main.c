#include <pic.h>

typedef unsigned char int8;

void main()
{
	OPTION=PS2|PS0;

	TMR0=0;			//reset timer 0

 	CMCON0 = 0b00000111;

	ADON=0;	
	ANS0=0;			//disable analog
	ANS1=0;			//disable analog
	ANS2=0;
	ANS3=0;

	TRISIO0=0;
	TRISIO1=1;
	TRISIO2=1;
	TRISIO3=1;
	TRISIO4=0;
	TRISIO5=0;

	T0IE=1;

	IRCF0=1;
	IRCF1=1;
	IRCF2=1;

	T1GE=0;
	T1OSCEN=0;
	TMR1CS=0;
	TMR1ON=1;
	T1CKPS0=0;
	T1CKPS1=1;

	PS0=1;
	//PS1=1;
	//PS2=1;

	TMR0=0;
	TMR1L=0;
	TMR1H=0;
	int8 last=0;

	while(1)
	{
		if(T0IF)
		{
			PS0=GPIO2?1:0;// only allow change in input at a edge
			T0IF=0;
		}
		if(GPIO1)
		{
			GPIO4=(TMR0&128)?1:0;
		}
		else GPIO4=0;

		int8 state = !GPIO3;
		if(state&&!last)//edge!
		{
			int delta=TMR1L;
			GPIO0=(delta&128)?1:0;
			TMR1H=0;
			TMR1L=0;
		}
		GPIO5=TMR1H?1:0;
		last=state;
	}
}