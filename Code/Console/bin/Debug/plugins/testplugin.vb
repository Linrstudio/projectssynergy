Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports Framework

Public Class plugin
    Shared outputs As List(Of DigitalOutput)
    <PluginEntry()> _
    Public Shared Sub Main()

        Console.WriteLine("w00tie iam in yer assembly")
        outputs.Add(New DigitalOutput("digital out 1", 1))
        outputs.Add(New DigitalOutput("digital out 2", 2))
        outputs.Add(New DigitalOutput("digital out 3", 3))
        outputs.Add(New DigitalOutput("digital out 4", 4))
        outputs.Add(New DigitalOutput("digital out 5", 5))
        outputs.Add(New DigitalOutput("digital out 6", 6))
        outputs.Add(New DigitalOutput("digital out 7", 7))
        outputs.Add(New DigitalOutput("digital out 8", 8))

        For Each o As DigitalOutput In outputs
            NetworkManager.LocalNode.AddNetworkClass(o)
        Next
    End Sub

    <PluginTick()> _
    Public Shared Sub Update()

    End Sub
End Class

Public Class DigitalOutput
    Inherits NetworkClassLocal
    <DllImport("k8055.dll")> _
    Private Shared Sub SetDigitalChannel(ByVal Channel As Integer)
    End Sub

    <DllImport("k8055.dll")> _
    Private Shared Sub ClearDigitalChannel(ByVal Channel As Integer)
    End Sub

    Public Channel As Integer

    Public Sub New(ByVal _Name As String, ByVal _Channel As Integer)
        MyBase.New(_Name)
        Channel = _Channel
    End Sub

    <NetworkField("On")> _
       Public enabled As Boolean

    <NetworkMethod("Update")> _
        Public Sub OnMemoryChanged()
        If enabled Then
            SetDigitalChannel(Channel)
        Else
            ClearDigitalChannel(Channel)
        End If
    End Sub
End Class
