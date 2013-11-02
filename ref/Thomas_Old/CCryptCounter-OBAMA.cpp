#include "CCryptCounter.h"

CCryptCounter::CCryptCounter() //default ctor
{

}

CCryptCounter::CCryptCounter(unsigned short in) //alternative ctor
{
    m_Counter = in;
}

unsigned char CCryptCounter::Key1()
{
    return (unsigned char)(m_Counter & 0xFF);
}

unsigned char CCryptCounter::Key2()
{
    return (unsigned char)(m_Counter >> 8);
}

CCryptCounter::~CCryptCounter()
{
    //dtor
}
