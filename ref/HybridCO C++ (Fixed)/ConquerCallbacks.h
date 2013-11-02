#ifndef _CONQUER_CALLBACKS_H_
#define _CONQUER_CALLBACKS_H_

class IMapObject;

extern int RemoveEntityCallback(IMapObject* Sender, IMapObject* Param, void* Arg);
extern int AddEntityCallback(IMapObject* Sender, IMapObject* Param, void* Arg);

#endif