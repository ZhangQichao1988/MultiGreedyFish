using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlRNGUtil
{
    private uint m_seed;
    private int m_position;
    public BlRNGUtil(uint seed)
    {
        m_seed = seed;
        m_position = 0;
    }
    
    //SRand-like Seed-related
    public void ResetSeed(uint seed, int position=0)
    {
        m_seed = seed;
        m_position = position;
    }

    public uint GetSeed()
    {
        return m_seed;
    }

    public void SetCurPos(int pos)
    {
        m_position = pos;
    }

    public int GetCurPos()
    {
        return m_position;
    }

    //Rand-like
    public uint RollRandomInt32()
    {
        uint v = (uint)GetNoiseHash(m_position++, m_seed);
        return v;
    }
    public short RollRandomInt16()
    {
        short v = (short)GetNoiseHash(m_position++, m_seed);
        return v;
    }
    public byte RollRandomByte()
    {
        byte v = (byte)GetNoiseHash(m_position++, m_seed);
        return v;
    }
    public uint RollRandomIntLessThan(uint maxValueNotInclusive)
    {
        uint v = (uint)GetNoiseHash(m_position++, m_seed);
        v = v % maxValueNotInclusive;
        return v;
    }
    public uint RollRandomIntInRange(uint minValueInclusive, uint maxValueNotInclusive)
    {
        uint v = (uint)GetNoiseHash(m_position++, m_seed);
        v = minValueInclusive + v % (maxValueNotInclusive - minValueInclusive);
        return v;
    }
    public float RollRandomFloatZeroToOne() { return 0; }
    public float RollRandomFloatInRange(float minValueInclusive, float maxValueNotInclusive) { return 0; }
    public bool RollRandomChance(float probabilityOfReturningTrue) { return true; }

    public ulong GetNoiseHash(int position, ulong seed = 0)
    {
        const ulong BIT_NOISE1 = 0xB5297A4D;
        const ulong BIT_NOISE2 = 0x68E31DA4;
        const ulong BIT_NOISE3 = 0x1B56C4E9;
        ulong mangled = (uint)position;
        mangled *= BIT_NOISE1;
        mangled += seed;
        mangled ^= (mangled >> 8);
        mangled += BIT_NOISE2;
        mangled ^= (mangled << 8);
        mangled *= BIT_NOISE3;
        mangled ^= (mangled >> 8);

        return mangled;
    }
}
