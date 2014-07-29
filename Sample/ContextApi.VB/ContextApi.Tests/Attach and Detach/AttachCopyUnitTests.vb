Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ContextApi.Model
Imports Telerik.OpenAccess

<TestClass()> _
Public Class AttachCopyUnitTests
    Inherits UnitTestsBase

    Private originalContext As EntitiesModel
    Private anotherContext As EntitiesModel

    <TestInitialize> _
    Public Overrides Sub TestInitialize()
        MyBase.TestInitialize()

        Me.originalContext = New EntitiesModel()
        Me.anotherContext = New EntitiesModel()
    End Sub

    <TestCleanup> _
    Public Overrides Sub TestCleanup()
        MyBase.TestCleanup()

        If Me.originalContext IsNot Nothing Then
            Me.originalContext.Dispose()
        End If

        If Me.anotherContext IsNot Nothing Then
            Me.anotherContext.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Scenario: Create a new instance of a persistent object and attach it to a context.
    ''' Outcome: The attached object will have ObjectState New and cause the context to which is attached to have changes.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_AttachNewInstance()

        Dim newEmployee As New Employee() With _
        { _
            .EmployeeId = 1337, _
            .FirstName = "Mad", _
            .LastName = "Jack" _
        }

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(newEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Me.anotherContext.SaveChanges()

        Assert.AreEqual(ObjectState.[New], attachedEmployeeState)
        Assert.IsTrue(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to another context.
    ''' Outcome: The managing context of the attached object will be the one to which it was attached and different than the one of the
    ''' original object.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_AttachToAnotherContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreEqual(Me.anotherContext, managingContext)
        Assert.AreNotEqual(Me.originalContext, managingContext)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to the same context.
    ''' Condition: There are no changes made to neither the original nor the detached objects.
    ''' Outcome: The attached object is the same as the original one and different from the detached. 
    ''' It will have ObjectState Clean and will not cause changes to the context to which is attached.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachCleanObject_AttachToTheSameContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.originalContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreSame(retrievedEmployee, attachedEmployee)
        Assert.AreNotSame(detachedEmployee, attachedEmployee)

        Assert.AreEqual(ObjectState.Clean, attachedEmployeeState)
        Assert.IsFalse(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to another context.
    ''' Condition: There are no changes to neither the original nor the detached objects.
    ''' Outcome: The attached object is different from the original and detached objects. 
    ''' It will have ObjectState Clean and will not cause changes to the context to which is attached.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachCleanObject_AttachToAnotherContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreNotSame(retrievedEmployee, attachedEmployee)
        Assert.AreNotSame(detachedEmployee, attachedEmployee)

        Assert.AreEqual(ObjectState.Clean, attachedEmployeeState)
        Assert.IsFalse(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to the same context.
    ''' Condition: Modify the original object before detaching it.
    ''' Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_ModifyOriginalObjectDetach_AttachToTheSameContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim newName As String = "Gary"
        retrievedEmployee.FirstName = newName

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        Me.originalContext.ClearChanges()
        Dim hasChangesBeforeAttach As Boolean = Me.originalContext.HasChanges

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.originalContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState)
        Assert.IsFalse(hasChangesBeforeAttach)
        Assert.IsTrue(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to another context.
    ''' Condition: Modify the original object before detaching it.
    ''' Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_ModifyOriginalObjectDetach_AndAttachToAnotherContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim newName As String = "Gary"
        retrievedEmployee.FirstName = newName

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState)
        Assert.IsTrue(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to the same context.
    ''' Condition: Modify the detached object before attaching it.
    ''' Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachModifyDetachedObject_AttachToTheSameContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        Dim newName As String = "Gary"
        detachedEmployee.FirstName = newName

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.originalContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState)
        Assert.IsTrue(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context and attach it to another context.
    ''' Condition: Modify the detached object before attaching it.
    ''' Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_Detach_ModifyDetachedObject_AttachToAnotherContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        Dim newName As String = "Gary"
        detachedEmployee.FirstName = newName

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState)
        Assert.IsTrue(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its original context and attach it to the same context.
    ''' Condition: Modify the original object after detaching it.
    ''' Outcome: The attached object will have ObjectState Dirty and will have the same value of the modified property as the original object.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_Detach_ModifyOriginalObject_AttachToTheSameContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim newName As String = "Gary"
        retrievedEmployee.FirstName = newName

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its original context and attach it to another context.
    ''' Condition: Modify the original object after detaching it.
    ''' Outcome: The attached object will have ObjectState Clean and the value of its respective property which was modified in the
    ''' original object will be unchanged. The context to which the object is attached will not have changes.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_Detach_ModifyOriginalObject_AttachToAnotherContext()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim newName As String = "Gary"
        retrievedEmployee.FirstName = newName

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreNotEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Clean, attachedEmployeeState)
        Assert.IsFalse(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its original context and attach it to the same context.
    ''' Condition: Modify the original after its detached copy has been attached.
    ''' Outcome: The attached object will have ObjectState Dirty and will have the same value of the modified property as the original object.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachObject_AttachToTheSameContext_ModifyOriginalObject()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(detachedEmployee)

        Dim newName As String = "Gary"
        retrievedEmployee.FirstName = newName

        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its original context and attach it to another context.
    ''' Condition: Modify the original object after detaching it.
    ''' Outcome: The attached object will have ObjectState Clean and the value of its respective property which was modified in the
    ''' original object will be unchanged. The context to which the object is attached will not have changes.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachObject_AttachToAnotherContext_ModifyOriginalObject()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)

        Dim newName As String = "Gary"
        retrievedEmployee.FirstName = newName

        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreNotEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Clean, attachedEmployeeState)
        Assert.IsFalse(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its original context and attach it to the same context.
    ''' Condition: Modify the detached object after it has been attached to a context.
    ''' Outcome: The attached object will have ObjectState Clean and the value of its respective property which has 
    ''' been modified in the detached object will not be changed.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachObject_AttachToTheSameContext_ModifyDetachedObject()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(detachedEmployee)

        Dim newName As String = "Gary"
        detachedEmployee.FirstName = newName

        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreNotEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Clean, attachedEmployeeState)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its original context and attach it to another context.
    ''' Condition: Modify the detached object after it has been attached to a context.
    ''' Outcome: The attached object will have ObjectState Clean and the value of its respective property which was modified in the
    ''' original object will be unchanged. The context to which the object is attached will not have changes.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachObject_AttachToAnotherContext_ModifyDetachedObject()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)

        Dim newName As String = "Gary"
        detachedEmployee.FirstName = newName

        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)
        Dim managingContext As OpenAccessContextBase = OpenAccessContext.GetContext(attachedEmployee)

        Assert.AreNotEqual(newName, attachedEmployee.FirstName)
        Assert.AreEqual(ObjectState.Clean, attachedEmployeeState)
        Assert.IsFalse(hasChangesAfterAttach)
    End Sub

    ''' <summary>
    ''' Scenario: Attach a new object to a context.
    ''' Condition: The new object has a reference to a related object.
    ''' Outcome: The related object will also be attached to the context.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_AttachGraphOfRelatedObjects()
        Dim newEmployee As New Employee() With _
        { _
            .EmployeeId = 1337, _
            .FirstName = "Mad", _
            .LastName = "Jack" _
        }

        Dim newManager As New Employee() With _
        { _
            .EmployeeId = 1338, _
            .FirstName = "Sane", _
            .LastName = "Jack", _
            .Title = "Grand Master Manager" _
        }
        newEmployee.Supervisor = newManager

        Dim attachedEmployee As Employee = Me.originalContext.AttachCopy(Of Employee)(newEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.originalContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreEqual(newManager.EmployeeId, attachedEmployee.Supervisor.EmployeeId)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context an attach it to another context.
    ''' Condition: To the detached object add a reference to a related object.
    ''' Outcome: The related object will also be attached to the context.
    ''' </summary>
    <TestMethod> _
    Public Sub AttachCopy_DetachObject_AttachGraphOfRelatedObjects()
        Dim retrievedEmployee As Employee = Me.originalContext.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Dim detachedEmployee As Employee = Me.originalContext.CreateDetachedCopy(Of Employee)(retrievedEmployee)

        Dim newManager As New Employee() With _
        { _
            .EmployeeId = 1338, _
            .FirstName = "Sane", _
            .LastName = "Jack", _
            .Title = "Grand Master Manager" _
        }
        detachedEmployee.Supervisor = newManager

        Dim attachedEmployee As Employee = Me.anotherContext.AttachCopy(Of Employee)(detachedEmployee)
        Dim hasChangesAfterAttach As Boolean = Me.anotherContext.HasChanges
        Dim attachedEmployeeState As ObjectState = Me.GetObjectState(attachedEmployee)

        Assert.AreEqual(newManager.EmployeeId, attachedEmployee.Supervisor.EmployeeId)
    End Sub

End Class