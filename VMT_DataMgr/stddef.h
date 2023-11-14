#ifndef __STDDEF_HEADER__
#define __STDDEF_HEADER__

#ifndef VMT_DATAMGR_EXPORTS
#define VMT_DATAMGR_EXPORTS
#endif 

#ifdef VMT_DATAMGR_EXPORTS
#define VMT_DATAMGR_API __declspec(dllexport)
#else
#define VMT_DATAMGR_API __declspec(dllimport)
#endif

//============================================================================
//= Macro definition
#ifndef CHECKFALSE
#define CHECKFALSE(x,r) if(!(x)) { return (r);}
#define CHECKTRUE(x,r) if((x)) { return (r);}
#endif

#ifndef SAFE_DELETE
#define SAFE_DELETE(x) if(x) { delete (x); (x) = NULL; }
#endif

#ifndef SAFE_DELETE_ARRAY
#define SAFE_DELETE_ARRAY(x) if(x) { delete[] (x); (x) = NULL; }
#endif


#endif
