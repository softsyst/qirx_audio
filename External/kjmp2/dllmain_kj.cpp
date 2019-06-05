/**
** QIRX - Software Defined Radio
** Copyright (C) 2017 Clem Schmidt, softsyst GmbH, http://www.softsyst.com
**
** This program is free software; you can redistribute it and/or modify
** it under the terms of the GNU General Public License as published by
** the Free Software Foundation; either version 2 of the License, or
** (at your option) any later version.
**
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License
** along with this program; if not, write to the Free Software
** Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
**
**/
#include "stdafx.h"
#include "kjmp2.h"


static kjmp2_context_t mp2;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

int  __cdecl mp2_initialize()
{
	kjmp2_init(&mp2);
	return 0 ;
}

int __cdecl mp2_getSampleRate(unsigned char* buf)
{
	return kjmp2_get_sample_rate(buf);
}

unsigned long __cdecl mp2_decode(unsigned char* buf, int len, short* pcm)
{
	unsigned long cnt = 0;
	cnt = kjmp2_decode_frame(&mp2, buf, pcm);
	return cnt;
}
