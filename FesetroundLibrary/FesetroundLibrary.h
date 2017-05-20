// FesetroundLibrary.h

#pragma once
#pragma STDC FENV_ACCESS on
#include <fenv.h>       /* fesetround, FE_* */
#include <stdio.h>
using namespace System;

namespace FesetroundLibrary {

	public ref class FesetRound
	{
	public:
		static void SET_FPU_DOWNWARD()
		{
			fesetround(FE_DOWNWARD);
			printf("Set mode to downward\n");
		}
		static void SET_FPU_UPWARD()
		{
			fesetround(FE_UPWARD);
		}
		static void SET_FPU_TONEAREST()
		{
			fesetround(FE_TONEAREST);
		}
		static void SET_FPU_TOWARDZERO()
		{
			fesetround(FE_TOWARDZERO);
		}


	};
}
