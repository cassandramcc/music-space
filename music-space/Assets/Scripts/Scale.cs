using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Root{
    CMajor,
    GMajor,
    DMajor,
    AMajor,
    EMajor,
    BMajor,
    FSMajor,
    CSMajor,
    GSMajor,
    DSMajor,
    ASMajor,
    FMajor,

    AMinor = 0,
    EMinor = 1,
    BMinor = 2,
    FSMinor = 3,
    CSMinor = 4,
    GSMinor = 5,
    DSMinor = 6,
    ASMinor = 7,
    FMinor = 8,
    CMinor = 9,
    GMinor = 10,
    DMinor = 11
}


public static class BaseNotes{
    static readonly List<int> baseNotes = new List<int>(){0,2,4,5,7,9,11};

    public static int[] ShiftNotesBasedOnRoot(int shift){
        List<int> baseNotesCopy = baseNotes;
        List<int> shiftedNotes = new List<int>();
        for (int i = 0; i < shift; i++){
            shiftedNotes.AddRange(new int[]{baseNotesCopy[4],baseNotesCopy[5],baseNotesCopy[6],baseNotesCopy[0],baseNotesCopy[1],baseNotesCopy[2],(baseNotesCopy[3]+1)%12});
            baseNotesCopy = shiftedNotes;
            shiftedNotes = new List<int>();
        }
        return baseNotesCopy.ToArray();
    }
}

public class Scale
{

    public int rootNote;
    public int[] notes;

    public Scale(Root root){
        rootNote = (((int)root) * 7) % 12;

        notes = BaseNotes.ShiftNotesBasedOnRoot((int)root);
    }

}
