﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Facebook;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class MovieController : Controller
    {
        private ApplicationDbContext _context;

        public MovieController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Authorize(Roles = RoleName.CanManageMovies)]
        public ActionResult New()
        {
            var genre = _context.Genres.ToList();

            var viewModel = new MovieViewModel
            {
                Genres = genre
            };

            return View("MoviesForm",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Save(Movie movie, HttpPostedFileBase movieImage)
        public ActionResult Save(Movie movie, HttpPostedFileBase Image)
        {
            if (Image != null)
            {
                movie.MovieImage = new byte[Image.ContentLength];
                Image.InputStream.Read(movie.MovieImage, 0, Image.ContentLength);
            }

            if (!ModelState.IsValid)
            {
                var viewModel = new MovieViewModel(movie)
                {
                    Genres = _context.Genres.ToList()
                };
                return View("MoviesForm", viewModel);
            }

            if (movie.Id == 0)
            {
                movie.AddedDate = DateTime.Now;
                movie.NumberAvailable = movie.NumberInStock;
                _context.Movies.Add(movie);
            }
            else
            {
                var movieInDb = _context.Movies.Single(m => m.Id == movie.Id);
                //TryUpdateModel(movieInDb);
                if (movie.MovieImage != null)
                {
                    movieInDb.MovieImage = movie.MovieImage;
                }

                movieInDb.Name = movie.Name;
                movieInDb.GenreId = movie.GenreId;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.NumberInStock = movie.NumberInStock;
                movieInDb.NumberAvailable = movie.NumberInStock;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Movie");
        }

        // Edit Data
        [Authorize(Roles = RoleName.CanManageMovies)]
        public ActionResult Edit(int id)
        {
            var movie = _context.Movies.SingleOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return HttpNotFound();
            }

            var viewModel = new MovieViewModel(movie)
            {
                Genres = _context.Genres.ToList()
            };

            return View("MoviesForm",viewModel);
        }

        //
        // GET: /Movies/Random
        //[Route("movies/released/{year}/{month}")]
        //[Route("movies/released/{year}/{month:regex(\\d{2}):range(1, 12)}")]
        public ActionResult Details(int id)
        {
            var movie = _context.Movies.Include(m => m.Genre).FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return HttpNotFound();

            return View(movie);
        }

        public ViewResult Index()
        {
            if(User.IsInRole(RoleName.CanManageMovies))
                return View("List");
            return View("ReadOnlyList");
        }
    }
}