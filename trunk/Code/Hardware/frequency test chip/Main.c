#include <pic.h>

#define PLCTX;
#define PLCRX;

#define TX;
#define RX;

int PLCRXBuffer;
int PLCRXIndex;



void main()
{
	GPWUF=0;
	//CWUF=0;

	OPTION=PS2|PS0;
	
	TMR0=0;			//reset timer 0
	
	ANS0=0;			//disable analog
	ANS1=0;			//disable analog
	TRIS=0b101011;	//set in/out-puts
	
	GP2=1;

	C1OUTEN=1;	//dont ouput result on C1OUT
	C1POL=1;	//dont invert polarity
	C1ON=1;		//enable comparator
	C1NREF=0;	//check against 0.6V
	C1PREF=1;	//use GP0 as reference
	C1WU=1;		//disable wake up on change

	while(1)
	{
		for(int i=0;i<255;i++)
		{
			for(int i2=0;i2<4;i2++)
			{
				GP2=1;
				asm("nop");
				GP2=0;
			}
		}
		GP4=!GP4;
	}

#if 1
	while(1)
	{
		for(int i=0;i<30;i++)
		{
			int s;
			do
			{
				s=0;
				for(int j=0;j<10;j++)
				{
					s+=C1OUT?1:0;
				}
			}
			while(s!=0&&s!=10);
		}
		GP2=!GP2;
/*
		int on = C1OUT;
		GP2=on;
		GP4=!GP4;
*/
	}
#else
	while(1)
	{
		for(int i=0;i<30;i++)
		{
			while(GP5);
			while(!GP5);
		}
		GP2=!GP2;
	}
#endif
}

#if 0
void DoFeedBack()
{
	
}

void main()
{
	GPWUF=0;
	CWUF=0;
	
	OPTION=PS2|PS0;
	
	TMR0=0;			//reset timer 0
	
	ANS0=0;			//disable analog
	ANS1=0;			//disable analog
	TRIS=0b110000;	//set in/out-puts

	GP2=1;
	
	int highcount=0;
	while(1)
	{
		while(!(TMR0<128))DoFeedBack();
		//faling edge
		while(  TMR0<128 )DoFeedBack();
		//rising edge
		if(PLCRX)
			highcount++;
		else
		{
			// we should now know how long the line was high
			if(highcount>6)//write eight, read seven to nine
			{
				//special thingy
			}
			if(highcount>3)//write five, read four to six
			{
				
			}
			if(highcount>0)//write two read one to three
			{
				
			}
			highcount==0;
		}
		
		
		//switch (highcount)...
	}
}
#endif