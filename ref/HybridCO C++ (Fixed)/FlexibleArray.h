#pragma once
#ifndef _FLEXIBLE_ARRAY_H_
#define _FLEXIBLE_ARRAY_H_
#include <windows.h>

template <typename TElement>
class CFlexibleArrayEnumerator
{
private:
	TElement* _items;
	int counter;
	int stopat;
public:
	CFlexibleArrayEnumerator(TElement* Elements, int Count)
	{
		_items = new TElement[Count];
		memcpy(_items, Elements, sizeof(TElement) * Count);
		counter = -1;
		stopat = Count;
	}
	~CFlexibleArrayEnumerator() 
	{ 
		delete[] _items; 
	}
	bool MoveNext()
	{
		if (counter < stopat)
		{
			counter++;
		}
		return false;
	}
	const TElement GetCurrent() { return _items[counter]; }
};

template <typename TElement>
class CFlexibleArray
{
private:
	TElement* _items;
	int _size;
	int _capacity;
	CRITICAL_SECTION _Session;
public:
	int GetCount() { return _size; }
	int GetCapacity() { return _capacity; }
	TElement* GetElements() { return _items; }
	CFlexibleArrayEnumerator<TElement>* GetEnumerator() { return new CFlexibleArrayEnumerator<TElement>(_items, _size); }
	void CopyTo(TElement* TArray) { memcpy(TArray, _items, _size * sizeof(TElement)); }

	__declspec(property(get = GetCount)) int Count;
	__declspec(property(get = GetCapacity)) int Capacity;
	__declspec(property(get = GetElements)) TElement* Elements;

	// These two functions (ObtainSyncHandle, FreeSyncHandle) are the alternative to GetEnumerator()
	// During while the loop handle is active, the collection will be unmodifiable
	// If you add an element, or remove an element (with Sync=true), this will result in thread deadlock!
	
	void ObtainSyncHandle() { ::EnterCriticalSection(&_Session); }
	void FreeSyncHandle() { ::LeaveCriticalSection(&_Session); }

	CFlexibleArray(void)
	{
		_items = new TElement[0];
		_size = 0;
		_capacity = 0;
		::InitializeCriticalSection(&_Session);
	}
	~CFlexibleArray(void)
	{
		delete[] _items;
		::DeleteCriticalSection(&_Session);
	}


	// These six functions here (Add, EnsureCapacity, RemoveAt, Remove(2), Clear)
	// are all completely thead-safe aslong as Sync=true
	// Once you call one of these functions without Sync=true, than
	// the burden of thread-synchronization is on you.

	void Add(TElement Element, bool Sync = true)
	{
		if (Sync)
			::EnterCriticalSection(&_Session);
		if (_size == _capacity)
		{
			EnsureCapacity(_size + 1, false);
		}
		_items[_size++] = Element;
		if (Sync)
			::LeaveCriticalSection(&_Session);
	}
	bool Contains(TElement Element, bool Sync = true)
	{
		bool found = false;
		if (Sync)
			::EnterCriticalSection(&_Session);
		for (int i = 0; i < _size; i++)
		{
			if (_items[i] == Element)
			{
				found = true;
				break;
			}
		}
		if (Sync)
			::LeaveCriticalSection(&_Session);
		return found;
	}
	void EnsureCapacity(int min, bool Sync = true)
	{
		if (Sync)
			::EnterCriticalSection(&_Session);
		if (_capacity < min)
		{
			int num = (_capacity == 0) ? 4 : (_capacity * 2);
			if (num < min)
			{
				num = min;
			}
			TElement* destinationArray = new TElement[num];
			memcpy(destinationArray, _items, _size * sizeof(TElement));
			delete[] _items;
			_items = destinationArray;
			_capacity = num;
		}
		if (Sync)
			::LeaveCriticalSection(&_Session);
	}
	// Returns false if index is out-of-range
	bool RemoveAt(int Index, bool Sync = true)
	{
		bool ret = false;
		if (Sync)
			::EnterCriticalSection(&_Session);
		if (Index > -1 && Index < _size)
		{
			_size--;
			memcpy(_items + Index, _items + Index + 1, (_size - Index) * sizeof(TElement));
			memset(&_items[_size], 0, sizeof(TElement));
			ret = true;
		}
		if (Sync)
			::LeaveCriticalSection(&_Session);
		return ret;
	}
	// Returns false if the element is not found
	bool Remove(TElement Element, bool Sync = true)
	{
		bool ret = false;
		if (Sync)
			::EnterCriticalSection(&_Session);
		for (int i = 0; i < _size; i++)
		{
			if (_items[i] == Element)
			{
				_size--;
				memcpy(_items + i, _items + i + 1, (_size - i) * sizeof(TElement));
				memset(&_items[_size], 0, sizeof(TElement));
				ret = true;
				break;
			}
		}
		if (Sync)
			::LeaveCriticalSection(&_Session);
		return ret;
	}
	// Returns false if the element is not found
	bool Remove(TElement* Element, bool Sync = true) 
	{
		bool ret = false;
		if (Sync)
			::EnterCriticalSection(&_Session);
		for (int i = 0; i < _size; i++)
		{
			if (memcmp(&_items[i], Element, sizeof(TElement)) == 0)
			{
				_size--;
				memcpy(_items + Index, _items + Index + 1, (_size - Index) * sizeof(TElement));
				memset(&_items[_size], 0, sizeof(TElement));
				ret = true;
				break;
			}
		}
		if (Sync)
			::LeaveCriticalSection(&_Session);
		return ret;
	}
	void Clear(bool Sync = true)
	{
		if (Sync)
			::EnterCriticalSection(&_Session);
		for (int i = 0; i < _size; i++)
		{
			memset(&_items[i], 0, sizeof(TElement));
		}
		_size = 0;
		if (Sync)
			::LeaveCriticalSection(&_Session);
	}
};

#endif
