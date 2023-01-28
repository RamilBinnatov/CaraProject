using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.Board;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BoardController : Controller
    {
        #region readonly
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public BoardController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Board> boards = await _context.Boards
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .ToListAsync();

            ViewBag.take = take;

            List<BoardListVM> mapDatas = GetMapDatas(boards);

            int count = await GetPageCount(take);

            Paginate<BoardListVM> result = new Paginate<BoardListVM>(mapDatas, page, count);

            return View(result);
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Board board = await _context.Boards.FirstOrDefaultAsync(m => m.Id == id);

                if (board is null) return NotFound();

                return View(new BoardEditVM
                {
                    Title = board.Title,
                    Description = board.Description,
                    Image = board.Image,
                });

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BoardEditVM board)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(board);
                }
                Board dbBoard = await GetByIdAsync(id);
                if (board.Photo != null)
                {
                    if (!board.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!board.Photo.CheckFileSize(20000))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }
                    string fileName = Guid.NewGuid().ToString() + "_" + board.Photo.FileName;
                    Board boardDb = await _context.Boards.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (boardDb is null) return NotFound();

                    if (boardDb.Image == board.Image)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/Hero", fileName);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await board.Photo.CopyToAsync(stream);
                    }

                    dbBoard.Image = fileName;

                }

                dbBoard.Title = board.Title;
                dbBoard.Description = board.Description;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }
        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardCreateVM board)
        {
            if (!ModelState.IsValid) return View();

            if (!board.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Please choose correct image type");
                return View();
            }

            if (!board.Photo.CheckFileSize(200000))
            {
                ModelState.AddModelError("Photo", "Please choose correct image size");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "_" + board.Photo.FileName;

            string path = Helper.GetFilePath(_env.WebRootPath, "assets/img/hero", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await board.Photo.CopyToAsync(stream);
            }

            Board newBoard = new Board
            {
                Title = board.Title,
                Description = board.Description,
                Image = fileName,
            };

            await _context.Boards.AddAsync(newBoard);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            Board board = await GetByIdAsync(id);

            if (board == null) return NotFound();

            string path = Helper.GetFilePath(_env.WebRootPath, "img", board.Image);


            Helper.DeleteFile(path);

            _context.Boards.Remove(board);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region SetStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<Board> dbBoard = await _context.Boards.Where(m => m.isActive).ToListAsync();

            if (dbBoard.Count < 1)
            {
                Board board = await _context.Boards.FirstOrDefaultAsync(m => m.Id == id);

                if (board is null) return NotFound();

                if (board.isActive)
                {
                    board.isActive = false;
                }
                else
                {
                    board.isActive = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                Board board = await _context.Boards.FirstOrDefaultAsync(m => m.Id == id);
                if (board.isActive)
                {
                    board.isActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }
        #endregion

        #region Services
        private async Task<Board> GetByIdAsync(int id)
        {
            return await _context.Boards.FindAsync(id);
        }

        private List<BoardListVM> GetMapDatas(List<Board> boards)
        {
            List<BoardListVM> boardListVMs = new List<BoardListVM>();

            foreach (var item in boards)
            {
                BoardListVM newBoard = new BoardListVM
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    IsActive = item.isActive,
                    Image = item.Image
                };

                boardListVMs.Add(newBoard);
            }

            return boardListVMs;
        }

        private async Task<int> GetPageCount(int take)
        {
            int boardCount = await _context.Boards.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)boardCount / take);
        }

        #endregion

    }
}
