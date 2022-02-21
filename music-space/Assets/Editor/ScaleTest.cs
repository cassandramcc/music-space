using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ScaleTest
{
    [Test]
    public void Scale_CreateObject_CorrectRoot(){
        Scale cm = new Scale(Root.CMajor);
        Assert.AreEqual(0,cm.rootNote);

        Scale gm = new Scale(Root.GMajor);
        Assert.AreEqual(7,gm.rootNote);

        Scale bm = new Scale(Root.BMajor);
        Assert.AreEqual(11,bm.rootNote);

        Scale fm = new Scale(Root.FMajor);
        Assert.AreEqual(5,fm.rootNote);
    }

    [Test]
    public void Scale_CreateNotes_CorrectNotes(){
        Scale cm = new Scale(Root.CMajor);
        Assert.AreEqual(new int[]{0,2,4,5,7,9,11}, cm.notes);
        Assert.AreEqual(7,cm.notes.Length);

        Scale gm = new Scale(Root.GMajor);
        Assert.AreEqual(new int[]{7,9,11,0,2,4,6},gm.notes);
        Assert.AreEqual(7,gm.notes.Length);

        Scale bm = new Scale(Root.BMajor);
        Assert.AreEqual(new int[]{11,1,3,4,6,8,10},bm.notes);
        Assert.AreEqual(7,bm.notes.Length);

        Scale fm = new Scale(Root.FMajor);
        Assert.AreEqual(new int[]{5,7,9,10,0,2,4},fm.notes);
        Assert.AreEqual(7,fm.notes.Length);

    }
}
