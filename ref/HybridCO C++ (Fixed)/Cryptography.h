#pragma once
#ifndef _CRYPTOGRAPHY_H_
#define _CRYPTOGRAPHY_H_

class CAuthCryptography
{
private:
	struct IOCounter
	{
		unsigned char Low;
		unsigned char High;
		
		void set_Counter(unsigned short counter) { *((unsigned short*)&Low) = counter; }
		unsigned short get_Counter() { return *((unsigned short*)&Low); }
		__declspec(property(put = set_Counter, get = get_Counter)) unsigned short Counter;
	};
	unsigned char* Key3;
	unsigned char* Key4;
	IOCounter In;
	IOCounter Out;
	bool UsingAlternate;
public:
	void Encrypt(void* in, void* out, int Length);
	void Decrypt(void* in, void* out, int Length);
	void SetKeys(DWORD* inKey1, DWORD* inKey2);

	CAuthCryptography(void);
	~CAuthCryptography(void);
};

class Assembler
{
public:
	static int RollRight(DWORD Value, char Roll, char Size);
};

class CPasswordCryptography
{

public:
	static void Decrypt(DWORD* Password);
};
#endif