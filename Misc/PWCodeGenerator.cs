/*
    Encode PW NFT and soul data into 32bit value with checksum
*/

using System;


public struct Species
{
    public bool hasPandroid;
    public bool hasCyborg;
    public bool hasSpectre;
    public bool hasXGene;
    public bool hasLegendary;
    public bool hasMagic;
}

public struct Careers
{
    public bool hasHacker;
    public bool hasTrader;
    public bool hasWarrior;
    public bool hasDruid;
}

public struct SoulScores
{
    public uint Intelligence;
    public uint Dexterity;
    public uint Strength;
    public uint WillPower;
}


static public class PWCodeGenerator
{

    static public (byte, uint) Generate(Species speciesOwned, Careers careersOwned, SoulScores soulScores, uint totalNFTs)
    {
        uint code = 0;
        byte checksum;

        code = EncodeSoulScores(soulScores);
        code = code << 8;      // Shift bits 16 spaces left
        code += EncodeSpeciesAndCareer(speciesOwned, careersOwned);
        code = code << 8;       // Shift bits 8 spaces left
        code += EncodeMisc(speciesOwned, totalNFTs);

        checksum = CalcChecksum(code);
        
        return (checksum, code);
    }

    static private byte EncodeMisc(Species speciesOwned, uint totalNFTs)
    {
        // Encode legendary/magic status, how many nfts in total are owned into a single byte
        byte tempByte = 0;

        if (speciesOwned.hasLegendary) tempByte += 1;
        if (speciesOwned.hasMagic) tempByte += 2;

        if (totalNFTs > 63) tempByte += 252;    // max value so as not to affect bit1 and bit2
        tempByte += (byte)(totalNFTs * 4);

        return tempByte;
    }

    static private byte EncodeSpeciesAndCareer(Species speciesOwned, Careers careersOwned)
    {
        // Encode each species and career as a single bit each
        byte tempByte = 0;

        if (careersOwned.hasHacker) tempByte += 1;
        if (careersOwned.hasDruid) tempByte += 2;
        if (careersOwned.hasTrader) tempByte += 4;
        if (careersOwned.hasWarrior) tempByte += 8;

        tempByte = (byte)(tempByte << 4);   // Shift bits 4 spaces left

        if (speciesOwned.hasPandroid) tempByte += 1;
        if (speciesOwned.hasCyborg) tempByte += 2;
        if (speciesOwned.hasXGene) tempByte += 4;
        if (speciesOwned.hasSpectre) tempByte += 8;

        return tempByte;

    }

    static private uint EncodeSoulScores(SoulScores soulScores)
    {
        uint temp32Bits = 0;

        temp32Bits = soulScores.Strength;
        temp32Bits = temp32Bits << 4;
        temp32Bits += soulScores.Intelligence;
        temp32Bits = temp32Bits << 4;
        temp32Bits += soulScores.WillPower;
        temp32Bits = temp32Bits << 4;
        temp32Bits += soulScores.Dexterity;

        return temp32Bits;
    }

    static private byte CalcChecksum(uint dataBlock)
    {
        byte checksum = 0;
        byte[] dataChunks = new byte[4];
        byte total = 0;

        // Split data block into four separate blocks
        dataChunks[0] = (byte)(dataBlock);
        dataChunks[1] = (byte)(dataBlock >> 8);
        dataChunks[2] = (byte)(dataBlock >> 16);
        dataChunks[3] = (byte)(dataBlock >> 24);

        foreach(byte chunk in dataChunks)
            total += chunk;

        checksum = (byte)(0xFF - total);    // checksum = value which must be added to sum of datachunks to == 0xFF

        return checksum;
    }
}
