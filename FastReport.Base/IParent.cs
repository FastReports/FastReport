using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport
{

  /// <summary>
  /// Implement this interface if your object can contain list of child objects.
  /// </summary>
  public interface IParent
  {
    /// <summary>
    /// Gets a value indicating that this object can contain the specified child object.
    /// </summary>
    /// <param name="child">Child object.</param>
    /// <returns><b>true</b> if this object can contain the specified child object; otherwise, <b>false</b>.</returns>
    bool CanContain(Base child);

    /// <summary>
    /// Gets a list of child objects.
    /// </summary>
    /// <param name="list">List to fill with values.</param>
    void GetChildObjects(ObjectCollection list);
    
    /// <summary>
    /// Adds a child object to this object's childs.
    /// </summary>
    /// <param name="child">Object to add.</param>
    void AddChild(Base child);
    
    /// <summary>
    /// Removes a specified object from this object's childs.
    /// </summary>
    /// <param name="child"></param>
    void RemoveChild(Base child);
    
    /// <summary>
    /// Returns z-order of the specified child object.
    /// </summary>
    /// <param name="child">Child object.</param>
    /// <returns>Z-order of the specified object.</returns>
    /// <remarks>
    /// This method must return the index of a specified child object in the internal child list.
    /// </remarks>
    int GetChildOrder(Base child);
    
    /// <summary>
    /// Sets the z-order of the specified object.
    /// </summary>
    /// <param name="child">Child object.</param>
    /// <param name="order">New Z-order.</param>
    /// <remarks>
    /// This method must place the specified child object at the specified position in the internal child list.
    /// </remarks>
    void SetChildOrder(Base child, int order);

    /// <summary>
    /// Updates the children layout when the size of this object is changed by dx, dy values.
    /// </summary>
    /// <param name="dx">X delta.</param>
    /// <param name="dy">Y delta.</param>
    /// <remarks>
    /// This method must update positions/sizes of child objects whose <b>Dock</b> or <b>Anchor</b> properties
    /// are set to non-default values.
    /// </remarks>
    void UpdateLayout(float dx, float dy);
  }
}
