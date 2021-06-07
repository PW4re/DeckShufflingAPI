using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DeckShuffling.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeckShuffling.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeckShufflingController : ControllerBase
    {
        private readonly ILogger<DeckShufflingController> _logger;
        private readonly IConfiguration _configuration;
        private Func<List<Card>, List<Card>> _shuffleAlgorithm;

        public DeckShufflingController(ILogger<DeckShufflingController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            InitShuffleAlgorithm();
        }

        private void InitShuffleAlgorithm()
        {
            var shuffleAlgorithmName = _configuration["ShuffleAlgorithm"];
            if (shuffleAlgorithmName != "simple" && shuffleAlgorithmName != "poker")
            {
                Console.WriteLine(
                    "В appsettings.json по ключу 'ShuffleAlgorithm' нужно установить одно из значений: simple, poker"
                );
                Environment.Exit(7);
            }

            _shuffleAlgorithm = Shuffler.SimpleShuffle;
            if (shuffleAlgorithmName == "poker") _shuffleAlgorithm = Shuffler.PokerStyleShuffle;
        }

        /// <summary>
        /// Получить колоду по имени (в её текущем состоянии)
        /// </summary>
        /// <param name="deckName">Часть URL (например, Sample_deck), слова разделены нижним подчёркиванием</param>
        [HttpGet("{deckName}")]
        public Deck GetDeck(string deckName) => DeckRepository.GetDeckByName(CreateModelDeckName(deckName));
        
        /// <summary>
        /// Получить список названий колод
        /// </summary>
        /// <returns>Список названий колод</returns>
        [HttpGet]
        public List<string> GetDeckList() => DeckRepository.GetDeckNameList();

        /// <summary>
        /// Удалить именованную колоду
        /// </summary>
        /// <param name="deckName">Часть URL (например, Sample_deck), слова разделены нижним подчёркиванием</param>
        /// <returns>
        /// Успешно ли прошло удаление колоды. Возвращает false, если колоды с таким именем нет в памяти
        /// </returns>
        [HttpDelete("{deckName}")]
        public bool DeleteDeck(string deckName) => DeckRepository.DeleteDeck(CreateModelDeckName(deckName));

        /// <summary>
        /// Создать именованную колоду.
        /// </summary>
        /// <param name="deckName">Часть URL (например, Sample_deck), слова разделены нижним подчёркиванием</param>
        /// <param name="initiatorPoco">
        /// json-объект, содержащий имя колоды и порядковые номера младшей и старшей карт (туз - 1 ... король - 13).
        /// </param>
        [HttpPost("{deckName}")]
        public Deck CreateNewDeck(string deckName, [FromBody]DeckInitiatorPoco initiatorPoco)
        {
            var deck = new Deck(
                initiatorPoco.name,
                (CardValues) initiatorPoco.startCardIndex,
                (CardValues) initiatorPoco.finishCardIndex
            );
            DeckRepository.AddDeck(deck);

            return deck;
        }

        /// <summary>
        /// Перетасовать колоду с указанным именем.
        /// </summary>
        /// <param name="deckName">Часть URL (например, Sample_deck), слова разделены нижним подчёркиванием</param>
        [HttpPut("{deckName}")]
        public void ShuffleDeck(string deckName) 
            => DeckRepository.ShuffleDeck(CreateModelDeckName(deckName), _shuffleAlgorithm);
        
        /// <summary>
        /// Преобразует urlDeckName в имя колоды в обычном формате (разделенное пробелами)
        /// </summary>
        /// <param name="urlDeckName">Часть URL (например, Sample_deck), слова разделены нижним подчёркиванием</param>
        /// <returns></returns>
        private static string CreateModelDeckName(string urlDeckName)
            => string.Join(' ', urlDeckName.Split('_'));

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class DeckInitiatorPoco
        {
            public string name { get; set; }
            
            public int startCardIndex { get; set; }
            
            public int finishCardIndex { get; set; }

            private DeckInitiatorPoco()
            {
                name = "null";
                startCardIndex = 1;
                finishCardIndex = 13;
            }

            public DeckInitiatorPoco(string name, int start, int finish)
            {
                this.name = name;
                if (start > finish) 
                    throw new ArgumentException("Parameter 'start' cannot be greater than 'finish'.");
                if (start < 1 || start > 13 || finish < 1 || finish > 13)
                    throw new ArgumentException("Parameters must take values of [1, 13].");
                startCardIndex = start;
                finishCardIndex = finish;
            }
        }
    }
}