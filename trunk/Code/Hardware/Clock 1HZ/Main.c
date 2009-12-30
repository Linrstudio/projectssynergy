#include <pic.h>

void main()
{
	GPWUF=0;
	CWUF=0;
	OPTION=PS2;
	TMR0=0;			//reset timer 0
	ANS0=0;			//disable analog
	ANS1=0;			//disable analog
	TRIS=0b110000;	//set in/out-puts
	while(1)GP2=TMR0>128;
}