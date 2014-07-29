Imports System
Imports System.Linq
Imports ContextApi.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.OpenAccess
Imports Telerik.OpenAccess.FetchOptimization
Imports ContextApi.Tests

''' <summary>
''' This class tests Adding, Added, Changing, Changed, Removing, Removed, Refreshing, Refreshed and ObjectConstructed events of the context.
''' </summary>
<TestClass()> _
Public Class TrackingChangesTests
    Inherits UnitTestsBase

    Private addingTagValue As Object = Nothing
    Private changingTagValue As Object = Nothing
    Private deletingTagValue As Object = Nothing
    Private refreshingTagValue As Object = Nothing

    Private addingEventFireCount As Integer
    Private addedEventFireCount As Integer
    Private changingEventFireCount As Integer
    Private changedEventFireCount As Integer
    Private removingEventFireCount As Integer
    Private removedEventFireCount As Integer
    Private refreshingEventFireCount As Integer
    Private refreshedEventFireCount As Integer
    Private objectConstructedEventFireCount As Integer

    <TestInitialize()> _
    Public Overrides Sub TestInitialize()
        MyBase.TestInitialize()

        Me.addingTagValue = New Object()
        Me.changingTagValue = New Object()
        Me.deletingTagValue = New Object()
        Me.refreshingTagValue = New Object()

        addingEventFireCount = 0
        addedEventFireCount = 0
        changingEventFireCount = 0
        changedEventFireCount = 0
        removingEventFireCount = 0
        removedEventFireCount = 0
        refreshingEventFireCount = 0
        refreshedEventFireCount = 0
        objectConstructedEventFireCount = 0
    End Sub

    <TestCleanup()> _
    Public Overrides Sub TestCleanup()
        Me.addingTagValue = Nothing
        Me.changingTagValue = Nothing
        Me.deletingTagValue = Nothing
        Me.refreshingTagValue = Nothing

        MyBase.TestCleanup()
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object is added to a context.
    ''' Adding event: Occurs before a persistent object is added to the object context.
    ''' Added event: Occurs after an object was marked as to be persisted by the object context.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Added()
        Using dbContext As New EntitiesModel()
            AddHandler dbContext.Events.Adding, AddressOf Events_Adding
            AddHandler dbContext.Events.Added, AddressOf Events_Added

            Try
                Dim _bug As New Bug()
                ' If the object is not attached, the events Adding and Added will not be fired for that object
                dbContext.Add(_bug)

                Assert.AreEqual(1, addingEventFireCount)
                Assert.AreEqual(1, addedEventFireCount)
            Finally
                RemoveHandler dbContext.Events.Adding, AddressOf Events_Adding
                RemoveHandler dbContext.Events.Added, AddressOf Events_Added
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object which is attached to a context is being changed.
    ''' Changing event: Occurs before a field of a persistent object is changed by the application.
    ''' Changed event: Occurs after a field of a persistent object has been changed by the application.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Updated()
        Using dbContext As New EntitiesModel()
            AddHandler dbContext.Events.Changing, AddressOf Events_Changing
            AddHandler dbContext.Events.Changed, AddressOf Events_Changed

            Try
                Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
                Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))

                ' Touch the impact to load it to the memory so in the event Changing the value of the
                ' e.OldValue will be the old value of the property - otherwise null
                ' int? impact =  bug.Impact

                ' Setting the impact will not load the old impact to the memory.
                _bug.Impact = 1

                Assert.AreEqual(1, changingEventFireCount)
                Assert.AreEqual(1, changedEventFireCount)
            Finally
                RemoveHandler dbContext.Events.Changing, AddressOf Events_Changing
                RemoveHandler dbContext.Events.Changed, AddressOf Events_Changed
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object which is attached to a context is being deleted.
    ''' Removing: Occurs before a persistent object is removed from the object context.
    ''' Removed:  Occurs after a persistent object was marked as to be removed from the object context.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Deleted()
        Using dbContext As New EntitiesModel()
            AddHandler dbContext.Events.Removing, AddressOf Events_Removing
            AddHandler dbContext.Events.Removed, AddressOf Events_Removed

            Try
                Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
                Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))
                dbContext.Delete(_bug)

                Assert.AreEqual(1, removingEventFireCount)
                Assert.AreEqual(1, removedEventFireCount)
            Finally
                RemoveHandler dbContext.Events.Removing, AddressOf Events_Removing
                RemoveHandler dbContext.Events.Removed, AddressOf Events_Removed
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object is constructed by a context.
    ''' ObjectConstructed event: Occurs after an object has been constructed by Telerik Data Access runtime.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Constructed()
        Using dbContext As New EntitiesModel()
            AddHandler dbContext.Events.ObjectConstructed, AddressOf Events_ObjectConstructed

            Try
                ' Here object gets constructed and ObjectConstructed event is fired
                Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
                Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))

                Assert.AreEqual(1, objectConstructedEventFireCount)
                Assert.IsNotNull(_bug)
            Finally
                RemoveHandler dbContext.Events.ObjectConstructed, AddressOf Events_ObjectConstructed
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object is constructed by a context.
    ''' ObjectConstructed event: Occurs after an object has been constructed by Telerik Data Access runtime.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Constructed_CreateDetachedCopy()
        Using dbContext As New EntitiesModel()
            Dim _project As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(_project, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            Dim strategy As New FetchStrategy()
            strategy.LoadWith(Of Project)(Function(p) p.Tasks)

            Try
                AddHandler dbContext.Events.ObjectConstructed, AddressOf TaskConstructed

                ' Here three tasks get constructed and ObjectConstructed event is fired three times.
                Dim detachedProject As Project = dbContext.CreateDetachedCopy(_project, strategy)

                Assert.AreEqual(3, objectConstructedEventFireCount)
                Assert.IsNotNull(detachedProject)
            Finally
                RemoveHandler dbContext.Events.ObjectConstructed, AddressOf TaskConstructed
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object is constructed by a context.
    ''' ObjectConstructed event: Occurs after an object has been constructed by Telerik Data Access runtime.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Constructed_GetObjectByKey()
        ' get bugId here from different context
        Dim bugId As Integer = 0
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))

            bugId = _bug.TaskId
        End Using

        Using dbContext As New EntitiesModel()
            AddHandler dbContext.Events.ObjectConstructed, AddressOf Events_ObjectConstructed

            Try
                ' Here object gets constructed and ObjectConstructed event is fired
                Dim bugObject As Bug = TryCast(dbContext.GetObjectByKey(New ObjectKey(GetType(Bug).FullName, bugId)), Bug)

                Assert.AreEqual(1, objectConstructedEventFireCount)
                Assert.IsNotNull(bugObject)
            Finally
                RemoveHandler dbContext.Events.ObjectConstructed, AddressOf Events_ObjectConstructed
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object is constructed by a context.
    ''' ObjectConstructed event is not fired if the object that is to be constructed is already cached.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Constructed_EventIsNotFired()
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))

            AddHandler dbContext.Events.ObjectConstructed, AddressOf ObjectConstructedIsNotFired

            Try
                ' ObjectConstructed event is not fired because the bug is cached.
                Dim bugObject As Bug = TryCast(dbContext.GetObjectByKey(New ObjectKey(GetType(Bug).FullName, _bug.TaskId)), Bug)
                bugObject = dbContext.CreateDetachedCopy(bugObject)
                bugObject = dbContext.Bugs.FirstOrDefault()
                Assert.IsNotNull(bugObject, MessageHelper.NoRecordsInDatabase(GetType(Bug)))

                Assert.AreEqual(0, objectConstructedEventFireCount)
            Finally
                RemoveHandler dbContext.Events.ObjectConstructed, AddressOf ObjectConstructedIsNotFired
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Track when an object is refreshed by a context.
    ''' Refreshing event: Occurs before a persistent object is refreshed from the database.
    ''' Refreshed event:  Occurs after a persistent object was refreshed from the database.
    ''' </summary>
    <TestMethod()> _
    Public Sub Object_Is_Being_Refreshed()
        Using dbContext As New EntitiesModel()
            AddHandler dbContext.Events.Refreshing, AddressOf Events_Refreshing
            AddHandler dbContext.Events.Refreshed, AddressOf Events_Refreshed

            Try
                Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
                Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))

                dbContext.Refresh(RefreshMode.OverwriteChangesFromStore, _bug)

                Assert.AreEqual(1, refreshingEventFireCount)
                Assert.AreEqual(1, refreshedEventFireCount)
            Finally
                RemoveHandler dbContext.Events.Refreshed, AddressOf Events_Refreshed
                RemoveHandler dbContext.Events.Refreshing, AddressOf Events_Refreshing
            End Try
        End Using
    End Sub

    Private Sub ObjectConstructedIsNotFired(ByVal sender As Object, ByVal e As ObjectConstructedEventArgs)
        Throw New ApplicationException("ObjectConstructed event should not be fired!")
    End Sub

    Private Sub TaskConstructed(ByVal sender As Object, ByVal e As ObjectConstructedEventArgs)
        Assert.IsTrue(TypeOf e.PersistentObject Is Task)
        objectConstructedEventFireCount += 1
    End Sub

    Private Sub Events_Refreshed(ByVal sender As Object, ByVal e As RefreshEventArgs)
        Assert.AreSame(refreshingTagValue, e.Tag)
        refreshedEventFireCount += 1
    End Sub

    Private Sub Events_Refreshing(ByVal sender As Object, ByVal e As RefreshEventArgs)
        e.Tag = refreshingTagValue
        refreshingEventFireCount += 1
    End Sub

    Private Sub Events_ObjectConstructed(ByVal sender As Object, ByVal e As ObjectConstructedEventArgs)
        ' The constructed object can be accessed via PersistentObject  property
        Assert.IsTrue(TypeOf e.PersistentObject Is Bug)
        objectConstructedEventFireCount += 1
    End Sub

    Private Sub Events_Removed(ByVal sender As Object, ByVal e As RemoveEventArgs)
        Assert.AreSame(deletingTagValue, e.Tag)
        removedEventFireCount += 1
    End Sub

    Private Sub Events_Removing(ByVal sender As Object, ByVal e As RemoveEventArgs)
        e.Tag = deletingTagValue
        removingEventFireCount += 1
    End Sub

    Private Sub Events_Changed(ByVal sender As Object, ByVal e As ChangeEventArgs)
        Assert.AreSame(Me.changingTagValue, e.Tag)
        changedEventFireCount += 1
    End Sub

    Private Sub Events_Changing(ByVal sender As Object, ByVal e As ChangeEventArgs)
        Assert.IsFalse(e.Cancel)
        Assert.AreEqual("Impact", e.PropertyName)
        Assert.IsFalse(e.Finished)
        Assert.AreEqual(e.NewValue, 1)
        Assert.IsNull(e.OldValue)
        Assert.IsNotNull(e.PersistentObject)
        Assert.IsNull(e.Tag)
        Assert.IsFalse(e.WasDirty)
        Assert.IsFalse(e.WasLoaded)

        e.Tag = Me.changingTagValue

        changingEventFireCount += 1
    End Sub

    Private Sub Events_Added(ByVal sender As Object, ByVal e As AddEventArgs)
        Assert.IsNotNull(e.Tag)
        Assert.IsFalse(e.Cancel)
        Assert.IsTrue(e.Finished)
        Assert.IsNotNull(e.PersistentObject)
        Assert.AreSame(Me.addingTagValue, e.Tag)

        addedEventFireCount += 1
    End Sub

    Private Sub Events_Adding(ByVal sender As Object, ByVal e As AddEventArgs)
        Assert.IsNull(e.Tag)
        Assert.IsFalse(e.Cancel)
        Assert.IsFalse(e.Finished)
        Assert.IsNotNull(e.PersistentObject)

        ' The tag property can be set to an arbitrary object. This object can be accessed in the Added event handler.
        e.Tag = Me.addingTagValue
        ' If we set the Cancel property to true, the object will be not added to the context.
        'e.Cancel = true;

        addingEventFireCount += 1
    End Sub
End Class

