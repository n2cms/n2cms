#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;

namespace N2.Persistence
{
    
    /// <summary>
    /// The repository is a single point for database operations. All 
    /// persistence operations on database should pass through here.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IRepository<TEntity> : IDisposable
    {
        /// <summary>
        /// Get the entity from the persistance store, or return null
        /// if it doesn't exist.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>Either the entity that matches the id, or a null</returns>
        TEntity Get(object id);

        /// <summary>
        /// Finds entitities from the persistance store with matching property value.
        /// </summary>
        /// <param name="propertyName">The name of the property to search for.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>Entities with matching values.</returns>
        IEnumerable<TEntity> Find(string propertyName, object value);

        /// <summary>
        /// Finds entitities from the persistance store with matching property values.
        /// </summary>
        /// <param name="propertyValuesToMatchAll">The property-value combinations to match. All these combinations must be equal for a result to be returned.</param>
        /// <returns>Entities with matching values.</returns>
        IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll);

        /// <summary>
        /// Finds entities from persistence store with matching parameter expression.
        /// </summary>
        /// <param name="parameters">The parameters to match.</param>
        /// <returns>Entities with matching values.</returns>
        IEnumerable<TEntity> Find(IParameter parameters);

        /// <summary>
        /// Finds entities from persistence store with matching parameter expression.
        /// </summary>
        /// <param name="parameters">The parameters to match.</param>
        /// <returns>Entities with matching values.</returns>
        IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties);

        /// <summary>
        /// Register the entity for deletion when the unit of work
        /// is completed. 
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Register te entity for save or update in the database when the unit of work
        /// is completed. (INSERT or UPDATE)
        /// </summary>
        /// <param name="entity">the entity to save</param>
        void SaveOrUpdate(TEntity entity);

        /// <summary>
        /// Check if any instance of the type exists
        /// </summary>
        /// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
        bool Exists();

        /// <summary>
        /// Counts the overall number of instances.
        /// </summary>
        /// <returns></returns>
        long Count();

        /// <summary>
        /// Counts the overall number of instances.
        /// </summary>
        /// <param name="parameters">The parameters to match.</param>
        /// <returns></returns>
        long Count(IParameter parameters);

        /// <summary>Flushes changes made to items in this repository.</summary>
        void Flush();

        /// <summary>Begins a transaction.</summary>
        /// <returns>A disposable transaction wrapper.</returns>
        ITransaction BeginTransaction();

        /// <summary>Gets an existing transaction or null if no transaction is running.</summary>
        /// <returns>A disposable transaction wrapper.</returns>
        ITransaction GetTransaction();
    }

    /// <summary>
    /// The repository is a single point for database operations. All 
    /// persistence operations on database should pass through here.
    /// </summary>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    [Obsolete("Use IRepository<TEntity>")]
    public interface IRepository<TKey, TEntity> : IRepository<TEntity>
    {
        /// <summary>
        /// Get the entity from the persistance store, or return null
        /// if it doesn't exist.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>Either the entity that matches the id, or a null</returns>
        TEntity Get(TKey id);

        /// <summary>
        /// Get the entity from the persistance store, or return null
        /// if it doesn't exist.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <typeparam name="T">The type of entity to get.</typeparam>
        /// <returns>Either the entity that matches the id, or a null</returns>
        T Get<T>(TKey id);
    }
}
