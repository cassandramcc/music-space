using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class NotesToTimesTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void NotesToTimes_CorrectNotesAndTimes(){
        GameObject paintTest = new GameObject();
        Painter p = paintTest.AddComponent<Painter>();

        Tuple<List<Double>, List<long>> result = p.NotesToTimes(new double[]{44,44,55,55,66});
        Assert.AreEqual(new double[]{44,55,66},result.Item1);
        Assert.AreEqual(new long[]{200,200,100},result.Item2);

        result = p.NotesToTimes(new double[]{44,55,77,66});
        Assert.AreEqual(new double[]{44,55,77,66},result.Item1);
        Assert.AreEqual(new long[]{100,100,100,100},result.Item2);

        result = p.NotesToTimes(new double[]{44,55,55,55,55,55,44});
        Assert.AreEqual(new double[]{44,55,44},result.Item1);
        Assert.AreEqual(new long[]{100,500,100},result.Item2);
        
        result = p.NotesToTimes(new double[]{44,44,77,77});
        Assert.AreEqual(new double[]{44,77},result.Item1);
        Assert.AreEqual(new long[]{200,200},result.Item2);
    }
}
