using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using BaseAPI.Database;
using BaseAPI.Entities;
using BaseAPI.Models;
using BaseAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
 
namespace BaseAPI
{
    public abstract class AbstractService<K, V> where K : AbstractModel where V : AbstractEntity
    {
        private readonly string STATUS = "status";
        protected readonly BaseDbContext dbContext;
        protected readonly IHttpContextAccessor httpContext;

        protected AbstractService(BaseDbContext baseDbContext, IHttpContextAccessor httpContext)
        {
            dbContext = baseDbContext;
            this.httpContext = httpContext;
        }

        public enum Actions
        {
            CREATE,
            UPDATE,
            DELETE
        }

        protected JwtSecurityToken getToken()
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(((string) httpContext.HttpContext?.Request.Headers["authorization"])?.Split(" ")[1]);
        }

        /**
       * Adds the data from the model to the database
       * @param model - The model containing the data to add to the database
       * @return - The primary key of the new record
       * @throws InrangeException - Thrown if the model is invalid
       */
        public Guid? add(K model)
        {
            preAddCheck(model);
            V entity = convertToEntityForAdd(model);

            entity.DateCreated = DateTimeOffset.Now;
            entity.DateModified = DateTimeOffset.Now;

            dbContext.Add(entity);
            dbContext.SaveChanges();

            postSaveProcessing(entity, model, Actions.CREATE);
            addAuditData(null, entity, Actions.CREATE);
            return entity.Id;
        }

        /**
     * Updates the database entry with the given Id with the data in the model provided
     * @param model - The model with the updated data
     * @param idStr - The id of the model to update
     * @throws InrangeException - Thrown if the model is invalid or if the row can't be updated
     */
        public void update(K model, String idStr)
        {
            Guid id = Guid.Parse(idStr);
            update(model, id);
        }

        /**
     * Loads an entity by the id sent in and converts it to a front end model.
     * @param idStr - The string representation of the ID
     * @return - The front end model of the entity.
     */
        public K getById(String idStr)
        {
            Guid id = Guid.Parse(idStr);
            V entity = getById(id);
            return convertToModel(entity);
        }

        /* =====================================================
     * Helper methods that can be overriden
     * =====================================================
     */

        /**
     * Updates the database entry with the given id with the data in the model provided
     * @param model - The model with the updated data
     * @param id - The primary key of the row to update
     */
        protected void update(K model, Guid id)
        {
            V entity = getById(id);
            V beforeEntity = entity.CloneJson();

            preUpdateCheck(entity, model);

            entity = populateEntityForUpdate(entity, model);
            entity.DateModified = DateTimeOffset.Now;

            dbContext.SaveChanges();

            postSaveProcessing(entity, model, Actions.UPDATE);
            addAuditData(beforeEntity, entity, Actions.UPDATE);
        }

        /**
         * Grabs the logged in users name from the Security context holder
         * @return - The logged in users name
         */
        // protected String loadLoggedInUser() {
        //     try {
        //         Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        //         return authentication.getName();
        //     } catch (Exception e) {
        //         return "N/A";
        //     }
        // } //TODO: figure this out

        /**
     * Loads an entity by the id sent in and converts it to a front end model.
     * @param id - The id
     * @return - The front end model of the entity.
     */
        protected V getById(Guid id)
        {
            return dbContext.Find<V>(id);
        }

        protected Paged<K> GetByPage(int limit, int page, Expression<Func<V, bool>> wherePredicate = null, Expression<Func<V, object>> orderPredicate = null)
        {
            Paged<V> entities = dbContext.Set<V>()
                .AsNoTracking()
                .OrderBy(orderPredicate ?? (entity => entity.DateCreated))
                .Where(wherePredicate ?? (entity => true))
                .Paginate(page, limit);

            return new Paged<K>()
            {
                CurrentPage = entities.CurrentPage,
                PageSize = entities.PageSize,
                TotalItems = entities.TotalItems,
                TotalPages = entities.TotalPages,
                Items = entities.Items.Select(convertToModelForList).ToList()
            };
        }


        /* =====================================================
         * Methods to be overriden if needed
         * =====================================================
         */

        /**
     * Any checks that we want to do before we update a record do here
     * @param entity - The entity we are updating
     * @param model - The model with the updated information
     * @throws InrangeException - Thrown if any checks aren't passed
     */
        protected void preUpdateCheck(V entity, K model)
        {
            // Gives a place do any additional checks before an update.
        }

        /**
     * Any checks that we want to do before we add a record do here
     * @param model - The model we are adding
     * @throws InrangeException - Thrown if any checks don't pass
     */
        protected void preAddCheck(K model)
        {
            // Gives a place do any additional checks before an add.
        }

        /**
     * If we need to do any processing after an entity is saved, do it here
     * @param entity - The updated entity
     * @param model - The model that was used to save
     * @param action - The action that was performed
     */
        protected void postSaveProcessing(V entity, K model, Actions action)
        {
            // This provides a place to do any post save processing for adding notifiable events or any other clean up
        }

        /**
     * If we need to and any audit records, do it here
     * @param beforeEntity - The entity before the update
     * @param afterEntity - The entity after the update
     * @param action - The action the user is performing
     */
        protected void addAuditData(V beforeEntity, V afterEntity, Actions action)
        {
            // This provides a place to do any auditing if we would want to
        }

        /* =====================================================
         * Abstract methods
         * =====================================================
         */
        /**
     * Converts the model to an Entity
     * @param model - The model to convert
     * @return - The entity representing the data
     */
        protected abstract V convertToEntityForAdd(K model);

        /**
     * Converts the model to an entity in order to use it as an example
     * @param model - The model to convert
     * @return - The entity representation of the model
     */
        protected abstract V convertToEntityForSearch(K model);

        /**
     * Used for an update, populate the entity sent in with the data from the model
     * @param entity - The entity to update
     * @param model - The model with the data to update
     * @return - The updated entity
     */
        protected abstract V populateEntityForUpdate(V entity, K model);

        /**
     * Converts the entity sent in to the model of the service
     * @param entity - The entity to convert
     * @return - The model representing the data
     */
        protected abstract K convertToModel(V entity);

        /**
     * Converts the entity sent in to the model for a list
     * @param entity - The entity to convert
     * @return - The model representing the data
     */
        protected abstract K convertToModelForList(V entity);
    }
}