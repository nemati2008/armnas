﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApi.Models.Internal;

namespace WebApi.Controllers
{

    [EnableCors]
    public class MessageController : ODataController
    {

        private readonly AppDbContext _context;

        public MessageController(DbContextOptions<AppDbContext> options)
        {
            _context = new AppDbContext(options);
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get() { return Ok(_context.Messages); }

        /// <exception cref="T:System.ArgumentNullException"></exception>
        [HttpGet]
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.Messages.AsQueryable().FirstOrDefault(c => c.Id == key));
        }



        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">An error is encountered while saving to the database.</exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
        ///                 A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///                 This is usually because the data in the database has been modified since it was loaded into memory.</exception>
        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] MessageODataHack payload)
        {
            if (payload == null)
                return BadRequest("Please put a valid object in request body.");
            var fixedPayload = new Message(payload) // hack, please check MessageODataHack summary.
            {
                HasBeenRead = false,
                Date = DateTime.Now
            };
            await _context.Messages.AddAsync(fixedPayload);
            await _context.SaveChangesAsync();
            return Created(payload);
        }

        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
        ///                 A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///                 This is usually because the data in the database has been modified since it was loaded into memory.</exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">An error is encountered while saving to the database.</exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.InvalidOperationException"></exception>
        [HttpPatch]
        [EnableQuery]
        public async Task<IActionResult> Patch([FromBody] MessageODataHack payload, int key)
        {
            if (payload == null || key == 0)
                return BadRequest("Please put valid object in request body.");
            var obj = await _context.Messages.AsQueryable().SingleAsync(c => c.Id == key);
            if (obj == null)
                return BadRequest("Object with this Id was not found.");
            obj.HasBeenRead = payload.HasBeenRead;
            await _context.SaveChangesAsync();
            return Updated(payload);
        }

    }

}
