using mui.Context.Protocol;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	/// <summary>
	/// What: genre and style of the audio or video media
	///  Why: the genre and the style can be used in the pathname to store the audio and/or video media file
	/// </summary>
	internal sealed class HandleMediaGenre
	{
		#region LOCAL VARIABLE
		/// <summary>
		/// What: List of the available Genre/Styles of the singleton
		///  Why: easily adds a new Genre/Styles in the list
		/// </summary>
		private List<MediaGenre> _Details = new List<MediaGenre>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		/// <summary>
		/// What: array of the Genre/Styles information
		///  Why: easily access a read-only array
		/// </summary>
		internal MediaGenre[] Details => _Details.ToArray();
		/// <summary>
		/// What: Access the available Styles from the Genre
		///  Why: The genre is a 1 to N to the style, this allow to have an easy simple access to the style when using the style in the destination pathname
		/// </summary>
		internal MediaGenre this[string label]
		{
			get
			{
				lock( Info )
					foreach( MediaGenre mg in Details )
						if( mg.Label == label )
							return mg;
				return null;
			}
		}
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>
		/// What: Singleton instance of the object
		///  Why: Allow all application's methods to access the singleton
		/// </summary>
		internal static HandleMediaGenre Info { get; private set; } = new HandleMediaGenre();
		#endregion SINGLETON

		#region PUBLIC METHODS
		/// <summary>
		/// What: Load the Genres/styles from the xml file and initialize the Genres/styles if not existing.
		///  Why: Allow to specifically populate the singleton instance when initializing the application.
		/// </summary>
		internal static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
			{
				LogTrace.Label();
				Info._Details = new List<MediaGenre>()
				{
					//	https://www.studiobinder.com/blog/movie-genres-list/#animation-genre
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Action" , Description = "Movies in the action genre are defined by risk and stakes.\nWhile many movies may feature an action sequence, to be appropriately categorized inside the action genre, the bulk of the content must be action-oriented, including fight scenes, stunts, car chases, and general danger.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Heroic Bloodshed" , Description = "This action sub-genre is defined by values like duty, brotherhood, honor, redemption, and the protection of the vulnerable. It was initially created in Hong Kong cinema but has since made its way around the world." },
							new MediaGenre.MediaStyle{ Label = "Military Action" , Description = "While some movies may incorporate various military characters, settings, themes, and events, this particular sub-genre focuses on their exploits and suggests these events are entertaining rather than tragic." },
							new MediaGenre.MediaStyle{ Label = "Espionage" , Description = "Espionage action movies are similar to military action movies in that they’re intended for excitement and entertainment rather than focusing on the political and psychological aspects of espionage." },
							new MediaGenre.MediaStyle{ Label = "Wuxia Action" , Description = "This highly specific sub-genre focuses on martial arts as both a form of excitement, but also as a chivalrous act of protection and honor." },
							new MediaGenre.MediaStyle{ Label = "Disaster" , Description = "Disaster movies are defined by a large amount of destruction, specifically from naturally occurring events, where characters try to survive. If an alien force is the force of destruction, the film will be categorized as science fiction rather than a straight disaster movie." },
							new MediaGenre.MediaStyle{ Label = "Adventure" , Description = "Movies in the adventure genre are defined by a journey, often including some form of pursuit, and can take place in any setting." },
							new MediaGenre.MediaStyle{ Label = "Superhero" , Description = "The superhero movie is defined by characters not only with supernatural abilities but using those abilities for altruistic purposes. If the film has superpowers that are used for questionable purposes, it would be more of a supernatural thriller versus a “superhero” movie." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Animation" , Description = "The animation genre is defined by inanimate objects being manipulated to appear as though they are living.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Traditional" , Description = "Traditional animation is defined by hand-drawn and painted images that are assembled to animate a cartoon that tells a story." },
							new MediaGenre.MediaStyle{ Label = "Stop Motion" , Description = "Stop motion animation is defined by taking real objects and adjusting them frame by frame to simulate motion and emotion. Stop motion refers to the style of photography, while stop motion such as claymation and sometimes puppet animation can fall into multiple sub-genres." },
							new MediaGenre.MediaStyle{ Label = "Claymation" , Description = "Claymation is a form of stop motion animation, except the subjects used are built specifically out of clay." },
							new MediaGenre.MediaStyle{ Label = "Cutout" , Description = "This is where shapes are cut out and placed on top of one another to make figures and settings, all used to tell a story." },
							new MediaGenre.MediaStyle{ Label = "Computer Generated Imagery" , Description = "CGI is the most common form of modern animation, where modeling programs and software are used to animate cartoons." },
							new MediaGenre.MediaStyle{ Label = "Puppetry" , Description = "Puppetry animation is where puppets, including hand, stick, shadow, ventriloquist, and marionettes are used to tell a story. " },
							new MediaGenre.MediaStyle{ Label = "Live-Action" , Description = "Live-action animation is where animation, of any kind, is mixed with real-life subjects to create a single world. " }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Comedy" , Description = "The comedy genre is defined by events that are intended to make someone laugh, no matter if the story is macabre, droll, or zany. Comedy can be found in most movies, but if the majority of the film is intended to be a comedy you may safely place it in this genre.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Action-Comedy" , Description = "The action-comedy sub-genre incorporates humorous actions within the action, using the exciting events in the story for laughs." },
							new MediaGenre.MediaStyle{ Label = "Dark Comedy" , Description = "Dark comedy (or Black Comedy) is defined by using attitudes and events that would normally be objectionable to set up humorous situations." },
							new MediaGenre.MediaStyle{ Label = "Romantic Comedy" , Description = "Romantic comedies (aka Rom-Coms) are defined by comedy derived from relationship frustrations that are intimate in nature." },
							new MediaGenre.MediaStyle{ Label = "Buddy Comedy" , Description = "A buddy comedy is defined by at least two individuals who we follow through a series of humorous events. Often their (platonic) relationship is the main source of comedy in the story." },
							new MediaGenre.MediaStyle{ Label = "Road Comedy" , Description = "Road comedies are defined by humorous situations derived from a journey along a set path, and often feature a set of stops and characters along the way that forces the protagonist(s) further down the road." },
							new MediaGenre.MediaStyle{ Label = "Slapstick Comedy" , Description = "Slapstick comedy is defined by humor derived from physical movement, harm, or frustration that requires little to no dialogue." },
							new MediaGenre.MediaStyle{ Label = "Parody" , Description = "I’ve decided to put parody, spoof, and satire next to one another because they’re often thought to be synonyms, but truthfully they are not. A parody mocks and specifically targets a single piece of art or connected body of work. A parody is more precise, and more limited." },
							new MediaGenre.MediaStyle{ Label = "Spoof" , Description = "A spoof is broader than a parody because it mocks an entire genre or collection of similar, but separate works. Where parody targets a specific piece of art or entertainment, spoofs target the entire genre." },
							new MediaGenre.MediaStyle{ Label = "Satire" , Description = "Satire movies are the broadest of the three in that it mocks overall ideas, vices, human nature, institutions, or any number of concepts that don’t necessarily have a specific connection to another piece of art." },
							new MediaGenre.MediaStyle{ Label = "Sitcom" , Description = "A sitcom (situational comedy) is defined by a set group of people who must navigate through humorous situations and misunderstandings. Sitcoms in the past were very often captured using multiple cameras on a soundstage, but it is by no means required." },
							new MediaGenre.MediaStyle{ Label = "Sketch Comedy" , Description = "Sketch comedy is defined by a collection of separate situations, with no inherent connection to each other, and can include the use of parody, satire, spoof, and many other comedy sub-genres." },
							new MediaGenre.MediaStyle{ Label = "Mockumentary" , Description = "Mockumentaries use the documentary format for parody, satire, or spoof. They don’t mock the format, but rather use the format to mock." },
							new MediaGenre.MediaStyle{ Label = "Prank" , Description = "The prank genre is defined by a mixture of real-life participants who are lead through a planned event without their knowledge. The orchestrators often have a premeditated intention to coerce foolishness or error from the participant for the sake of humor or surprise." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Cooking" , Description = "All the cooking video",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Every day" , Description = "Every day cooking recipes." },
							new MediaGenre.MediaStyle{ Label = "Connoisseur" , Description = "Top end culinary recipes." },
							new MediaGenre.MediaStyle{ Label = "Tips" , Description = "cooking tips and recipes for remedy, conserve, ..." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Crime" , Description = "The crime genre deals with both sides of the criminal justice system but does not focus on legislative matters or civil suits and legal actions. The best crime movies often occupy moral gray areas where heroes and villains are much harder to define. ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Caper" , Description = "The caper sub-genre is defined by a group of criminals, often non-violent, who set out on a heist or job. A caper is often humorous and less serious in nature when compared to the other crime sub-genres." },
							new MediaGenre.MediaStyle{ Label = "Heist" , Description = "The heist sub-genre is defined by a criminal, or group of criminals, who set out to steal something valuable, and have a more serious tone when compared to a caper story. The subjects must navigate a set of obstacles and avoid law enforcement, and often the “getaway” is incorporated." },
							new MediaGenre.MediaStyle{ Label = "Gangster" , Description = "A gangster story follows and explores the world of organized crime. A film may include organized crime, but if the majority of the story doesn't explore organized crime, it wouldn’t fall into this sub-genre." },
							new MediaGenre.MediaStyle{ Label = "Cop (Police)" , Description = "The cop sub-genre follows a street cop (not a detective) who deals with criminals, crime, and the overall lifestyle as an officer of the law. You might find that some lists will have cop movies and detective movies intertwined, but for our list, we’ll focus on the beat-cops." },
							new MediaGenre.MediaStyle{ Label = "Detective" , Description = "A detective story follows an investigator or set of investigators, either private or as a representative of a government, and follows the clues and revelations of a particular case, or set of cases." },
							new MediaGenre.MediaStyle{ Label = "Courtroom" , Description = "The courtroom sub-genre requires the majority of the story to take place inside, or support the events that are connected to a court case." },
							new MediaGenre.MediaStyle{ Label = "Procedural" , Description = "A procedural is defined by following the established day-to-day events of investigating, solving, and prosecuting crime. Procedurals often end in situations where law enforcement has learned a valuable lesson, but their lives may not be irrevocably changed from each particular case. " }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Documentaries" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Scientific" , Description = "All lessons or courses on-line",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Informatic" , Description = "Computer and language related course" },
							new MediaGenre.MediaStyle{ Label = "Physics" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Mathematic" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Security" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Biology" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Chemistry" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Philosophy" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "popularization" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Epistemology" , Description = "all related to neuro science, epistemology, AI" }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Drama" , Description = "The drama genre is defined by conflict and often looks to reality rather than sensationalism. Emotions and intense situations are the focus, but where other genres might use unique or exciting moments to create a feeling, movies in the drama genre focus on common occurrences.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Melodrama" , Description = "A modern melodrama is defined by the prioritization of dramatic rhetoric and plot over character. The events are intended to elicit an intense emotional response. A melodrama strives for situations used to illustrate a larger moral thesis that acts as an agent of empathy." },
							new MediaGenre.MediaStyle{ Label = "Teen Drama" , Description = "The teen drama sub-genre is both simple and redundant. It focuses on the lives of teenagers, group dynamics, and general woes of adolescence." },
							new MediaGenre.MediaStyle{ Label = "Philosophical Drama" , Description = "The philosophical sub-genre is defined by an exploration of the human condition, and the drama is derived from the questions that are presented by mere existence and life itself." },
							new MediaGenre.MediaStyle{ Label = "Medical Drama" , Description = "The medical sub-genre focuses on the inherent drama of health conditions, the inner workings of hospitals, the relationship between doctors and medical staff, and the medical industry. There are medical procedurals that follow the day-to-day life of health care professionals." },
							new MediaGenre.MediaStyle{ Label = "Legal Drama" , Description = "The legal-drama sub-genre is defined by lawyers, judges, and legal complications that may be peripheral but not enveloped by the criminal justice system or matters relating to crime and punishment. While a legal drama may dip into criminal justice matters, the real focus is on characters at a law firm or judges chambers rather than a crime." },
							new MediaGenre.MediaStyle{ Label = "Political Drama" , Description = "The political-drama sub-genre focuses on the complications and inherent drama that takes place inside the world of politics. This can range anywhere from local government to national political climates." },
							new MediaGenre.MediaStyle{ Label = "Anthropological Drama" , Description = "The anthropological sub-genre focuses on the drama derived from human behavior and society at large, and while the story may feature a central protagonist, the story might focus on a specific culture or a broad representation of society." },
							new MediaGenre.MediaStyle{ Label = "Religious Drama" , Description = "The religious sub-genre is similar to the previous categories in that it focuses on the questions and inherent drama derived from religious situations and has the ability to incorporate supernatural events." },
							new MediaGenre.MediaStyle{ Label = "Docudrama" , Description = "A docudrama takes real-life accounts and recreates them in a way that attempts to accurately represent events while also realizing the dramatic potential of those events. Docudramas are held to a higher standard of accuracy (not quality) than historical accounts or memoirs." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Experimental" , Description = "The experimental genre is often defined by the idea that the work of art and entertainment does not fit into a particular genre or sub-genre, and is intended as such. Experimental art can completely forego a cohesive narrative in exchange for an emotional response or nothing at all. ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Surrealist" , Description = "Surrealism cannot be stylistically defined, and this is the point of the sub-genre itself. The intention of surrealist art is to act as an activity to broaden horizons, either of oneself or of others. Surrealist art often uses irrational imagery to activate the subconscious mind." },
							new MediaGenre.MediaStyle{ Label = "Absurdist" , Description = "The absurdist sub-genre focuses on characters who experience situations that suggest there is no central purpose to life. Another way to frame it is a set of experiences that catalyze a descent into nihilism." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Fantasy" , Description = "The fantasy genre is defined by both circumstance and setting inside a fictional universe with an unrealistic set of natural laws. The possibilities of fantasy are nearly endless, but the movies will often be inspired by or incorporate human myths.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Contemporary" , Description = "A contemporary fantasy story introduces elements of fantasy into or around a world that closely resembles the time period when it was conceived. Urban fantasy can serve as contemporary fantasy but must take place in an urban setting whereas contemporary fantasy can be set anywhere that resembles the corresponding time period." },
							new MediaGenre.MediaStyle{ Label = "Urban " , Description = "An urban fantasy is a story introduces elements of fantasy and is set entirely in an urban environment. The urban environment can be real, fictional, modern, or inspired by history, but the story must take place and deal with concepts and themes related to an urban environment." },
							new MediaGenre.MediaStyle{ Label = "Dark " , Description = "A dark fantasy is a story where elements of fantasy are introduced into a hostile and frightening world. If a significant portion of the story takes place in a world that has a range of circumstances, mood, and tone it would most likely be categorized as a high fantasy or general fantasy." },
							new MediaGenre.MediaStyle{ Label = "High " , Description = "High fantasy can also be referred to as epic fantasy, and introduces elements of fantasy in a fictional setting, and will include romance, battles, and mythical creatures. High fantasy is the fantasy genre equivalent of a historical epic or a science fiction space opera." },
							new MediaGenre.MediaStyle{ Label = "Myth" , Description = "A myth is defined by a story that often plays a fundamental role in the development of a society, which may include the origin story for humanity and existence. Often this will include characters that are gods, demigods, and supernatural humans. As noted by Joseph Campbell theory on The Hero's Journey, myths have similar characteristics despite an apparent lack of influence, which gives a myth the ability to be universally accepted." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Historical" , Description = "The historical genre can be split into two sections. One deals with accurate representations of historical accounts which can include biographies, autobiographies, and memoirs. The other section is made up of fictional movies that are placed inside an accurate depiction of a historical setting.\nThe accuracy of a historical story is measured against historical accounts, not fact, as there can never be a perfectly factual account of any event without first-hand experience. ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Historical Event" , Description = "The historical event genre focuses on a story that creates a dramatized depiction of an event that exists in popular accounts of history. This is different from a biography in that it focuses on an event." },
							new MediaGenre.MediaStyle{ Label = "Biography" , Description = "A biography (or biopic) is a story that details the life and is told by someone other than the subject. A biography will often span a large portion of the subject's life, but in some rare cases, it may focus on the time period where that person’s life had the greatest effect on history and society." },
							new MediaGenre.MediaStyle{ Label = "Historical Epic" , Description = "A historical epic is the dramatized account of a large scale event that has an attached historical account. They often feature battles, romance, and journeys, and will commonly revise history or provide assumptions that fill in gaps in the account of the historical event." },
							new MediaGenre.MediaStyle{ Label = "Historical Fiction" , Description = "Historical fiction takes place during a historical time period, and will often take a more liberal approach to representing history for the sake of drama and entertainment. Historical fiction may use real-life events and people to build context, but they’re meant to be accepted as a supposition rather than serve as an accurate historical account. " },
							new MediaGenre.MediaStyle{ Label = "Period Piece" , Description = "The difference between a period piece and historical fiction is slight, but the main difference is a general omission or a lack of necessity for real-life characters or events to provide context. Period pieces are merely defined by taking place in, and accurately depicting the time period as opposed to specific lives, events, or accounts. " },
							new MediaGenre.MediaStyle{ Label = "Alternate History" , Description = "Alternate history is defined by the rewriting of historical events for the sake of speculative outcomes. These movies commonly focus on important, highly influential moments that often lead to alternate futures. Some of these movies may even include supernatural elements." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Horror" , Description = "The horror genre is centered upon depicting terrifying or macabre events for the sake of entertainment. A thriller might tease the possibility of a terrible event, whereas a horror film will deliver all throughout the film. The best horror movies are designed to get the heart pumping and to show us a glimpse of the unknown. ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Ghost" , Description = "A ghost movie uses the spirit or soul of a deceased creature to introduce elements of horror. These movies can take place in any time period and are only required to evoke terror through the use of ghosts." },
							new MediaGenre.MediaStyle{ Label = "Monster" , Description = "A monster movie uses a deformed or supernatural creature or set of creatures, to introduce elements of horror. These movies can also take place in any time period or setting, and their only real requirement is that the antagonist is can be categorized as a monster." },
							new MediaGenre.MediaStyle{ Label = "Werewolf" , Description = "A werewolf movie introduces elements of horror through the use of a human or set of humans that transform into a wolf-like creatures. Sometimes these werewolves have the ability to shape-shift at will, but in other cases, their transformation is dictated by a full moon." },
							new MediaGenre.MediaStyle{ Label = "Vampire" , Description = "A vampire movie introduces elements of horror through the use of undead, immortal creatures that drink blood. They can be set in any time and place and must only use vampires as the antagonist. Some vampire movies feature vampires as the protagonist, but this is often used to build sympathy rather than as a device for terror." },
							new MediaGenre.MediaStyle{ Label = "Occult" , Description = "Occult movies are defined by an extension of pure reason and use paranormal themes to introduce elements of horror. Occult literally translates into “hidden from view” and involves the study of a deeper spiritual reality that extends scientific observation." },
							new MediaGenre.MediaStyle{ Label = "Slasher" , Description = "A slasher story introduces elements of horror through an antagonist or set of antagonists who stalk and murder a group of people, most commonly through the use of a blade or a sharp weapon. The slasher movie is so engrained in our movie culture, even non-slasher movies use some of the same techniques and tropes." },
							new MediaGenre.MediaStyle{ Label = "Splatter" , Description = "A splatter story introduces elements of horror by focusing on the vulnerability of the human body, and an emphasis on gore. Splatter movies often involve torture and attempt to present gore as an art form." },
							new MediaGenre.MediaStyle{ Label = "Found Footage" , Description = "Found footage can be used for any genre, but it is most commonly used in horror and features footage that appears to be an existing and informal recording of events with the purpose of simulating real-life horrific events." },
							new MediaGenre.MediaStyle{ Label = "Zombie" , Description = "The zombie movie has roots all the way back to the '30s but it didn't really kick into high gear until the late 1960s. The general plot of the best zombie movies involves a group of characters trying to survive in a world overrun by zombies. The specific cause for the event ranges from infectious disease to experimental drugs gone wrong." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Romance" , Description = "The romance genre is defined by intimate relationships. Sometimes these movies can have a darker twist, but the idea is to lean on the natural conflict derived from the pursuit of intimacy and love.  ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Drama" , Description = "The romance-drama sub-genre is defined by the conflict generated from a  romantic relationship. What makes a romance-drama different from a romantic-thriller is both the source of the drama but also the intentions and motivations that drive each character’s perspective." },
							new MediaGenre.MediaStyle{ Label = "Thriller" , Description = "The romance-thriller sub-genre is defined by a suspenseful story that includes and is most likely based around a romantic relationship. Some romantic thrillers can divert into psychological thrillers where the relationship is used to manipulate, but most focus on the characters attempting to make it out of events so that they may be together." },
							new MediaGenre.MediaStyle{ Label = "Period " , Description = "A period-romance story is defined by the setting and can include and incorporate other romance sub-genres. The setting must be a historical time period, and often will adhere to the societal norms of the specific time period, though some movies have taken a more revisionist approach." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Science Fiction" , Description = "Science fiction movies are defined by a mixture of speculation and science. While fantasy will explain through or make use of magic and mysticism, science fiction will use the changes and trajectory of technology and science. Science fiction will often incorporate space, biology, energy, time, and any other observable science.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Post-Apocalyptic" , Description = "Post-apocalyptic movies are based around the occurrence, effects, and struggle generated by an apocalyptic event. While a dystopian story may incorporate a large war or apocalyptic event in its narrative history, it will include a centralized government that was formed after the event. Apocalyptic movies will not have a centralized government but may feature smaller societies and tribes as part of the story." },
							new MediaGenre.MediaStyle{ Label = "Utopian" , Description = "The utopian genre is defined by the creator’s view of an idyllic world since each person has a unique view of what they deem to be the absence of struggle and incident, but generally, themes included in the movies are peace, harmony, and a world without hunger or homelessness. In the past, utopian movies have been tied to satire because the nature of a story is often conflict, and a utopian society is viewed as an unrealistic concept." },
							new MediaGenre.MediaStyle{ Label = "Dystopian" , Description = "A dystopian story is one that features a world or society that serves as a contradiction to an idyllic world. Often there is a centralized and oppressive government or religion that dictates the value of citizens on a dehumanizing level, and may or may not incorporate a destructive event that drove the creation of that centralized institution." },
							new MediaGenre.MediaStyle{ Label = "Cyberpunk" , Description = "The cyberpunk sub-genre is defined by a mixture of a desperate society oversaturated with the crime that takes place in a high tech world that includes cybernetic organisms, virtual reality, and artificial intelligence." },
							new MediaGenre.MediaStyle{ Label = "Steampunk" , Description = "The steampunk sub-genre is inspired by technology created during the 19th century and the industrial revolution and may be set in a speculative future, alternate universe, or revision of the 1800s." },
							new MediaGenre.MediaStyle{ Label = "Tech Noir" , Description = "Tech noir is similar to dystopian but defined by technology as the main source behind humanity's struggle and partial downfall. There is no requirement for a centralized government, and the only true aspect that places a story in this category is that technology threatens our reality." },
							new MediaGenre.MediaStyle{ Label = "Space Opera" , Description = "A space opera is defined by a mixture of space warfare, adventure, and romance. The genre got its name from similarities to “soap operas” and “horse operas” due to their collective connection to melodrama. The term “space opera” has no connection to the music of any kind." },
							new MediaGenre.MediaStyle{ Label = "Contemporary" , Description = "A contemporary science fiction story is set in the actual time period of its conception and introduces some form of a theoretical technology or scientific concept to serve as the story’s main source of conflict. This is different from tech-noir both due to scale and a strict time period." },
							new MediaGenre.MediaStyle{ Label = "Military" , Description = "A military science fiction story is defined by a strict focus on the military conflict in a speculative or future setting. While other movies may include space warfare, a military science fiction story will be limited to themes and events directly tied to military service and battle." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Thriller" , Description = "A thriller story is mostly about the emotional purpose, which is to elicit strong emotions, mostly dealing with generating suspense and anxiety. No matter what the specific plot, the best thrillers get your heart racing.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Psychological" , Description = "A psychological thriller focuses and emphasizes the unstable psychological state of the characters inside the story. Often there is a mysterious set of circumstances, and a paranoia, warranted or otherwise, that catalyzes extreme actions from the characters. " },
							new MediaGenre.MediaStyle{ Label = "Mystery" , Description = "A mystery story can often be connected to the crime genre, but may not involve or use law enforcement or the justice system as the main characters or backdrop for the story. A mystery story is defined by the plot, and both the character’s and the viewer’s relationship with the motivations and reality behind the events that occur. " },
							new MediaGenre.MediaStyle{ Label = "Techno" , Description = "The techno-thriller sub-genre is defined by a conflict that takes place for or through Various forms of technology. What makes a techno-thriller different from various action sub-genres is the level of detail paid toward the underlying technical aspects of the technology and its effects. " },
							new MediaGenre.MediaStyle{ Label = "Film Noir" , Description = "Some consider the definition of film noir to more of a style than a genre, because there is no requirement to be connected to a crime. There is, however, a natural overlap between style and genre in the best Film Noir movies. The central theme behind the noir sub-genre is a psychic imbalance that leads to self-hatred, aggression, or sociopathy." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Western" , Description = "Westerns are defined by their setting and time period. The story needs to take place in the American West, which begins as far east as Missouri and extends to the Pacific ocean. They’re set during the 19th century, and will often feature horse riding, military expansion, violent and non-violent interaction with Native American tribes, the creation of railways, gunfights, and technology created during the industrial revolution. ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Epic" , Description = "The idea of an epic western is to emphasize and incorporate many if not all of the western genre elements, but on a grand scale, and also use the backdrop of large scale real-life events to frame your story. " },
							new MediaGenre.MediaStyle{ Label = "Empire" , Description = "These movies follow a protagonist or a group of protagonists as they forge a large scale business based on natural resources and land. It can also follow the creation of the railroad, or large scale settlement. " },
							new MediaGenre.MediaStyle{ Label = "Marshal" , Description = "A marshal western is where we follow a lawman as they attempt to track down, apprehend, and punish a criminal or group of gangsters." },
							new MediaGenre.MediaStyle{ Label = "Outlaw" , Description = "An outlaw western is where we follow a criminal or group of criminals as they attempt crimes and evade the law. Often, these movies will portray the outlaws in a somewhat favorable manner." },
							new MediaGenre.MediaStyle{ Label = "Revenge" , Description = "This genre is defined by a singular goal and will incorporate the elements of the western genre while the protagonist seeks revenge. " },
							new MediaGenre.MediaStyle{ Label = "Revisionist" , Description = "A revisionist western challenges and often aims to disprove the notions propped up by traditional westerns. Early westerns often had their own agenda, and revisionist westerns attempt to dissolve and cast aside a commonly one-sided genre." },
							new MediaGenre.MediaStyle{ Label = "Spaghetti" , Description = "The Spaghetti Western genre was named such because the films were initially made in Italy or produced by Italian filmmakers. Because these films are defined by their ‘heritage’ they can also fall into many of the other western genres as long as they are Italian built." }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Video , Label = "Others" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Musical" , Description = "Musicals originated as stage plays, but they soon became a favorite for many film directors and have even made their way into television. Musicals can incorporate any other genre, but they incorporate characters who sing songs and perform dance numbers." },
							new MediaGenre.MediaStyle{ Label = "War " , Description = "The war genre has a few debatable definitions, but we’re going to try to be as straightforward and impartial as humanly possible. Movies in the war genre center around large scale conflicts between opposing forces inside a universe that shares the same natural laws as our own." },
							new MediaGenre.MediaStyle{ Label = "Biopics " , Description = "A movie genre that has been around since the birth of cinema, biopics are a category all their own. Biopics can technically run the gamut of movie genres (Sports movies, War, Westerns, etc.) but they often find their home in dramas. At their core, biopics dramatize real people and real events with varying degrees of verisimilitude." }
						}
					},
					//	https://strongsounds.com/blog/most-popular-genres-of-music/
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Classical" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Opera" , Description = "Opera is a type of classical music that tells a story using musical and non-musical elements. Opera singers are trained to use their voices to great effect, and many operas feature very beautiful music." },
							new MediaGenre.MediaStyle{ Label = "Choral Music" , Description = "Choral music is a type of classical music that uses the voices of a group of singers, called a choir, to create harmonies. Choral music often features religious themes, but it can also be secular." },
							new MediaGenre.MediaStyle{ Label = "Orchestral Music" , Description = "Orchestral music is a type of classical music that is performed by an orchestra, which is a large group of musicians who play string, wind, and percussion instruments. Orchestral music can be very exciting, or it can be very peaceful." },
							new MediaGenre.MediaStyle{ Label = "Symphonic Music" , Description = "Symphonic music is a type of orchestral music that is designed to be heard as a complete work. A symphony is usually between 30 and 60 minutes long and has four different parts, or movements." },
							new MediaGenre.MediaStyle{ Label = "Baroque" , Description = "Baroque music is a type of European classical music which was popular in the 1600s and 1700s. It is characterized by complex harmonies, elaborate ornamentation, and grandiose rhetoric. Many famous composers such as Johann Sebastian Bach, Antonio Vivaldi, and George Frideric Handel wrote music in the Baroque style." },
							new MediaGenre.MediaStyle{ Label = "Romantic " , Description = "Romantic music is a period of Western classical music that began in the late 18th or early 19th century. It is related to Romanticism, the Western artistic and literary movement that arose in the second half of the 18th century, and Romantic music in particular dominated the Romantic movement in Germany.\r\n\r\nIn the Romantic period, music became more expressive and emotional, expanding to encompass different styles, forms, and traditions. Composers sought to increase emotional expression and power through different means, including changing the form and feel of their pieces. While some Romantic composers opposed the ideology of earlier classical periods, others expanded it." },
							new MediaGenre.MediaStyle{ Label = "Modern" , Description = "Over the past few centuries, classical music has undergone a tremendous evolution, leading to the development of many different genres and sub-genres. While some purists may argue that certain genres are not “true” classical music, the fact remains that these various styles have all emerged from the same musical tradition.\r\n\r\nOne of the most significant changes in classical music occurred in the early 20th century, when composers began experimenting with new harmonic and dissonant sounds. This marked the beginning of “modern classical” music, which continued to evolve throughout the 20th century. Some of the most famous modern classical composers include Igor Stravinsky, Arnold Schoenberg, and Alban Berg.\r\n\r\nToday, modern classical music is still being composed and performed by musicians all over the world. If you’re interested in exploring this genre further, we recommend checking out some of the following modern classical composers:" }
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Blues" , Description = "It’s hard to pin down the exact origin of the blues, but it’s generally understood that it evolved from the field hollers and work songs of African American workers in southern states following the American civil war, which ended in 1865.\r\n\r\nThe early music was infused with many devices common to African music such as string bending, ‘call and response’ phrasing and a wavy, nasal vocal style.\r\n\r\nThe direct emotional outpouring also associated with early blues was also influenced by the ‘Spiritual’ – a form of religious song developed as part of the protestant religious revival in the US during the 19th century.\r\n\r\nBy the time the term ‘Blues’ was first used, the music was already quite close to what we understand it to be today and the first recordings weren’t made until the 1920s when the style was already established.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Classic Female Blues" , Description = "Many of the stars of early blues music were female vocalists who were hugely influential in bringing the blues to a wider audience and popularszing the music.\r\n\r\nCombining traditional folk blues with elements of vaudeville theatre, these vocalists were often accompanied either by a single pianist or a small jazz ensemble.\r\n\r\nThese artists were amongst the first blues musicians ever to be recorded and releases featuring legendary vocalists such as Ma Rainey, Mamie Smith, Ethel Waters and Bessie Smith dominated the genre in the 1920s." },
							new MediaGenre.MediaStyle{ Label = "Delta Blues" , Description = "The blues originating in the Mississippi delta is one of the oldest known forms of blues and also one of the the first to be captured on record.\r\n\r\nAlthough it’s possible that larger ensembles performed this music live, the earliest recordings primarily featured solo performers singing and accompanying themselves on a guitar – sometimes with harmonica as well.\r\n\r\nThe use of slide or bottleneck on the guitar was common, as was a heavy attack on the guitar strings which complemented the intensity of the lyrics.\r\n\r\nCommon themes were love and loss, life in the fields or on the road, and musings on religious salvation or damnation. \r\n\r\nKey figures to make Delta Blues recordings were Willie Brown, Charley Patton, and Son House and they influenced many later players including the legendary Robert Johnson – who legend has it sold his soul to Satan at the crossroads in exchange for his musical ability." },
							new MediaGenre.MediaStyle{ Label = "Chicago Blues" , Description = "The great migration of the 1920s saw many black musicians moving north to cities in search of work and a better life.\r\n\r\nThe blues began to evolve in this new environment and developments in different cities are often grouped together under the umbrella terms ‘Urban Blues’ or ‘Electric Blues’.\r\n\r\nThe style developed in Chicago was probably the most influential.\r\n\r\nAs the music was played in busy clubs, guitarists began to use amplification and experiment with overdrive and distortion, as well as performing with rhythm sections of drums, bass and sometimes piano.\r\n\r\nBlues musicians such as Muddy Waters, Howlin’ Wolf and John Lee Hooker all continued the development of this style and the Chicago sound became internationally known in the 1950s and 60s, influencing early rock’n’roll artists such as Bo Diddley and Chuck Berry." },
							new MediaGenre.MediaStyle{ Label = "Boogie Woogie" , Description = "One of the few styles to feature blues piano players as the primary instrumentalist, Boogie Woogie was established by pianists in Chicago in the 30s and early 40s.\r\n\r\nMusicians such as Jimmy Yancey, Albert Ammons, Pete Johnson and Meade Lux Lewis took the propulsive feel of stride and ragtime piano and developed driving ostinatos for their left hand, providing a foundation for the right hand to play melodic phrases and improvise solo lines." },
							new MediaGenre.MediaStyle{ Label = "Memphis Blues" , Description = "As well as early guitar blues and the compositions of W C Handy (who wrote many of the songs made famous by Classic Female Blues singers), ‘jug bands’ were a popular phenomenon on the early Memphis Blues scene.\r\n\r\nJug blues fused some of the syncopated rhythms of early jazz with a variety of folk music to create an energetic and highly-danceable style.\r\n\r\nAlongside the ubiquitous guitar, it also featured instruments such as harmonica, violin, mandolin and banjo.\r\n\r\nAlso, homemade instruments such as comb and paper, washboards and the jugs blown to create the bass sound which gives the style its name.\r\n\r\nLater, many blues musicians relocated to Memphis due to the blossoming scene based around Beale Street and the style changed significantly.\r\n\r\nB.B. King, Ike Turner and Howlin’ Wolf (before he moved to Chicago) were just a few of the musicians who performed and recorded in Memphis." },
							new MediaGenre.MediaStyle{ Label = "Jump Blues" , Description = "Evolving from Boogie Woogie and the big band sound of the 1940s, Jump Blues is an uptempo style which combines elements of blues music and swing.\r\n\r\nThe music is energetic and commonly features full rhythm sections and brass or woodwind instruments.\r\n\r\nSaxophonist Louis Jordan is arguably this style’s most famous exponent and it’s often considered a precursor to both R’n’B and rock’n’roll." },
							new MediaGenre.MediaStyle{ Label = "West Coast Blues" , Description = "T-Bone Walker was born in Texas, but moved to LA in the 1940s. He probably did more to popularise the use of electric guitar in this style than any other individual blues musician.\r\n\r\nHe was also largely responsible for developing the style now known as West Coast Blues, which incorporates elements of urban, jump and jazzy blues and often features the piano prominently. \r\n\r\nThe smooth, soulful vocal style of West Coast Blues is often similar to that of R’n’B and the guitar solos have a more noticeable jazz influence than some other types of the Blues.\r\n\r\nOther notable West Coast Blues musicians include Pee Wee Crayton, Charles Brown, Big Mama Thornton and Johnny ‘Guitar’ Watson." },
							new MediaGenre.MediaStyle{ Label = "New Orleans Blues" , Description = "As you might have guessed by now, many types of blues music is named after the city it was born in, and this one is no different!\r\n\r\nAlthough generally thought of as the birthplace of jazz, the great cultural melting-pot of New Orleans also gave rise to its own unique style of the blues.\r\n\r\nIncorporating Latin and Caribbean influences, New Orleans Blues has a unique syncopated rhythmic feel and piano, not guitar, is often the main chordal instrument.\r\n\r\nArtists such as Professor Longhair, James Booker and Dr. John are key figures in New Orleans Blues." },
							new MediaGenre.MediaStyle{ Label = "Texas Blues" , Description = "There has been a long tradition of blues music in Texas dating back to the 1900s.\r\n\r\nArtists such as Blind Lemon Jefferson made recordings in the 1920s which became hugely influential for many later musicians.\r\n\r\nIn the late 1960s and early 1970s Texas enjoyed a thriving blues scene based in the clubs of Austin.\r\n\r\nIncorporating influences from rock and country music, Texas Blues from this era focused heavily on guitar solos, often accompanied by bands featuring keyboards and horn sections.\r\n\r\nStevie Ray Vaughan emerged from this scene to achieve mainstream recognition with his virtuosic guitar playing and is regarded by many as one of the most influential blues musicians of all time – and one of the greatest guitarists in any genre." },
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Country" , Description = "Country music can be traced back to the beginning of the twentieth century. It was created mainly in the south of the USA, by working-class people. These people would use country as a means to tell stories through music, about the realities of everyday life and their perspectives.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Jazz" , Description = "In the early 20th century, musicians in the city of New Orleans experimented by blending musical elements from European and African genres. This resulted in the origination of jazz, which would go on to become one of the most popular and unique musical styles in existence.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Pop" , Description = "Pop music is perhaps the most obvious addition to this list. It could be said that pop is a compilation of other genres, which are considered to be mainstream in a certain time period.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Funk" , Description = "Like the soul, funk was also the result of African Americans blending jazz and R&B. Funk has a strong rhythmic pulse, prominent bass lines, and syncopated rhythm guitar playing." },
							new MediaGenre.MediaStyle{ Label = "Reggae" , Description = "Reggae was invented in Jamaica in the late 60s, and quickly become the country’s favorite music genre. In the decades that followed, it reached the UK, USA, and Africa, where it amassed huge audiences." },
							new MediaGenre.MediaStyle{ Label = "Disco" , Description = "Disco music rose to prominence in the late 60s and early 70s, making its way into the upmarket nightclubs of major US cities." },
							new MediaGenre.MediaStyle{ Label = "Pop Rock" , Description = "Pop rock is one of the most common types of pop music. It is a softer and more commercial version of rock with more emphasis on songwriting, recording, and mixing and less emphasis on attitude and musicality. It is less instrumental and easier to sing along to than rock music.\r\n\r\nPop rock’s roots go back to the 50s when it emerged from rock n’ roll as a softer alternative. It was an upbeat variety of rock music with more catchy melodies, repeated choruses, and softer instrumentation.\r\n\r\nArtists like the Beatles, Peter Frampton, and The Bells are some old-school pop-rock examples, while the likes of Maroon 5, The Script, and OneRepublic are recent examples." },
							new MediaGenre.MediaStyle{ Label = "Electropop" , Description = "One of the recent and popular pop music styles is electropop, which fuses electronic and pop music. It is considered a version of synth-pop with heavier use of electronic sounds. Today, heavy use of electronic sounds became a standard in pop music, and electropop has become the dominant style.\r\n\r\nWhile electropop’s history can be traced back to the 80s and the rise of synth pop with names like Gary Numan, the Human League, Soft Cell, and John Foxx, the style was revived by Britney Spears in 2007 with her “Blackout” album. Later, many artists followed that path, like Lady Gaga, Rihanna, Billie Eilish, and many more." },
							new MediaGenre.MediaStyle{ Label = "Teen Pop" , Description = "As the name suggests, Teen Pop is a subgenre of pop music targeted at teenagers. Teen Pop is an umbrella term as teen pop songs can have strong influences from R&B, dance, electronic, hip hop, and rock.\r\n\r\nTeen Pop has the characteristics of mainstream pop music, such as choreographed dances, heavy importance on visual appeal, and lyrics on relatable topics such as love, relationships, party culture, dancing, friendship, etc. The style of teen pop is relatively easy-listening, with auto-tuned vocals, heavy repetitions on choruses, and simple melodies.\r\n\r\nMany pop music artists can be considered Teen Pop, at least with some of their songs. Rihanna, Kylie Minogue, Spice Girls, Justin Bieber, and Backstreet Boys are some of the names that come to mind." },
							new MediaGenre.MediaStyle{ Label = "Hip Pop (Pop Rap)" , Description = "As a fusion of hip-hop and pop music, hip-pop or pop rap mixes catchy melodies and choruses with rhythm-based vocals. Pop rap was highly popular in the 90s with famous names like Run-DMC, LL Cool J, and Beastie Boys. Oftentimes, the choruses are similar to pop, while the verses are rapped.\r\n\r\nThe sub-genre was developed during the late 80s and got stronger each year as many artists dominated the pop-rap scene. After the 2000s, bands like Black Eyed Peas and artists like Drake, Nicki Minaj, Pitbull, and many more made hip pop one of the most popular pop styles." },
							new MediaGenre.MediaStyle{ Label = "Bubblegum" , Description = "Defined by simple childish lyrics, basic chords, and melodies, as well as harmonized vocals, Bubblegum Pop is an upbeat and catchy blend of rock n’ roll and pop music. The subgenre has its roots in the late 60s, targeting children and adolescents. \r\n\r\nThe first Bubblegum Pop hit was “Sugar, Sugar” by the fictional rock band The Archies. Most Bubblegum hits are one-time-wonders, and the music was considered disposable music.\r\n\r\nThe subgenre remained a commercial force until the late 70s as the characteristics of the genre, including upbeat tempo patterns, catchy hooks, and simple song structures, were carried into punk rock." },
							new MediaGenre.MediaStyle{ Label = "Country Pop" , Description = "As a result of country and pop music fusion, Country Pop uses string instruments and folk harmonies of country music along with catchy melodies, simple chords, and repetitive choruses of pop music. Some notable artists in the subgenre are Miley Cyrus, Taylor Swift, Kacey Musgraves, and Billy Currington.\r\n\r\nCountry Pop began to take shape in the 1950s when the Nashville sound was founded to reform the country sound to reach a wider audience, especially younger listeners. By the 70s, many country artists were already ranked in the pop lists. Therefore, the fusion was completed." },
							new MediaGenre.MediaStyle{ Label = "Folk Pop" , Description = "Originating from the 1960s, Folk Pop mixes folk and pop music by using pop structures, backing bands, commercial sound, and less guitar for shorter and softer songs, along with catchy melodies and repetitive choruses. It can be described as the softer version of folk rock which was popular in the 60s.\r\n\r\nThe genre was highly popular in the 60s and the 70s due to artists such as Simon And Garfunkel, Cat Stevens, Donovan, and Joni Mitchell." },
							new MediaGenre.MediaStyle{ Label = "Sunshine Pop" , Description = "Sunshine Pop is another pop subgenre that came to the music scene in the 60s in Southern California. It is a highly easy listening genre with lush vocals and light arrangements with lyrics around an appreciation for the world’s beauty. Some artists in the subgenre are The Turtles and the Association. \r\n\r\nSunshine Pop was a side branch of folk rock and California Sound movements. Most bands in the genre tried to follow the footsteps of bigger bands, such as The Beach Boys and The Mamas & the Papas. Sunshine Pop had its peak in the 60s all over the world." },
							new MediaGenre.MediaStyle{ Label = "House Pop" , Description = "House Pop is a loosely defined term that signals the dance-pop music that uses the ways of House music production. It is not an exact mix of pop and a house music sub-genre as it has characteristics such as the dominance of vocals, highly melodic structures, polished synths, and clean beats.\r\n\r\nHouse pop has a chill vibe instead of the euphoric feeling of heavier EDM genres. House Pop can be similar to any House music subgenre, such as Deep House, Garage House, Future House, Ambient House, and more.\r\n\r\nBob Sinclair, David Guetta, and Avicii are some examples, even though not all of their songs fall into this category." },
							new MediaGenre.MediaStyle{ Label = "Pop-R&B" , Description = "Often called Contemporary R&B, Pop-R&B is an old pop style that fuses rhythm and blues with different influences like pop, electronic, funk, and soul. The genre is characterized by the heavy use of drum-machine rhythm grooves, pitch-corrected vocals, and a distinctive production style.\r\n\r\nThe subgenre had its roots in the late 1970s when hip-hop was just coming to the music scene. Later in the 80s, a version of hip-hop emerged without the grit and roughness and more elements from soul, pop, and funk.\r\n\r\nNames like Curtis Mayfield, Marvin Gaye, and Stevie Wonder opened the way, while Beyonce, Usher, Alicia Keys, and many more significantly contributed to the genre." },
							new MediaGenre.MediaStyle{ Label = "Jazz Pop" , Description = "Jazz pop is a pop style that borrows elements and influences from Jazz music. The complex jazz instrumentation gives way to a more melodic and easy-listening format with a more vocal or instrument-centered structure. Also, jazz pop allows for only short improvisation parts, unlike jazz.\r\n\r\nSome important jazz-pop musicians are Kenny G, Bob James, and George Benson. The melody and the swing of jazz music are key to the style, along with the soft production, commercially viable, and radio-friendly arrangements." },
							new MediaGenre.MediaStyle{ Label = "Orchestral Pop" , Description = "One of the lesser-known pop music styles is Orchestral Pop, which is pop music composed for and played by an orchestra. Mainly, orchestral pop consists of symphonic performances of popular music like “Yesterday” by The Beatles, which was one of the first examples of the style.\r\n\r\nIn the late 60s, many producers started experimenting in unconventional ways and worked on orchestral arrangements of the releases of their artists. Some examples are Burt Bacharach and the Beach Boys’ Brian Wilson from the past, while The Last Shadow Puppets is a more recent example." },
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Rock" , Description = "One of the top musical genres in existence, rock music originated in the 1940s and 50s in the form of rock & roll. However, its roots can be traced back to the rhythm and blues of the African-American culture in the 1920s, merged with country music.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Punk Rock" , Description = "Punk rock has an aggressive sound, with fast-paced tempos and simple guitar riffs often played using only downstrokes. It was seen as a departure from the technical styles of the main music genres that had dominated the 1970s, with bands like The Ramones, The Sex Pistols, and The Clash bringing it into the public eye." },
							new MediaGenre.MediaStyle{ Label = "Grunge" , Description = "In the 1980s in Seattle, a collective of aspiring musicians had become disillusioned with the mainstream rock music that dominated the radio stations. Out of their frustration, a new genre was born – grunge.As the genre amassed a growing fan base, bands like Nirvana and Pearl Jam were propelled to stardom. Highly distorted guitar riffs, pounding drums and gritty vocals are three common characteristics of this genre. It continues to inspire the new generation of rock musicians today." },
							new MediaGenre.MediaStyle{ Label = "Alternative Rock" , Description = "Alternative rock had its breakthrough in the 80s as, you guessed it, an alternative to rock as people knew it at the time. The subgenre has many of the characteristics of classic rock, in addition to elements from other rock subgenres or even totally unrelated genres like hip-hop.\r\n\r\nAlternative rock is defined by edgy lyrics and experimental use of instrumentation. However, the subgenre remains loosely defined, to the point that it’s used to refer to any music that sounds “similar to rock ‘n’ roll”.\r\n\r\nSome of the most popular alternative rock bands include Nirvana, Red Hot Chili Peppers, and Sonic Youth." },
							new MediaGenre.MediaStyle{ Label = "Rock ‘n’ Roll" , Description = "The emergence of rock ‘n’ roll is often associated with youth revolt and rejection of social norms and gender discrimination. Rock ‘n’ roll songs are full of energy, have catchy melodies, and usually integrate elements from other music genres, like country and R&B. \r\n\r\nRock ‘n’ roll is one of the earliest subgenres of rock music, spreading throughout the late 1940s to early 1950s in the United States. AC/DC, Led Zeppelin, and The Rolling Stones are some of the rock ‘n’ roll giants that heavily contributed to its development. \r\n\r\nThe early works of rock’n’roll music typically used the saxophone or piano as the lead instrument. However, later on, these instruments were either replaced or used alongside the guitar." },
							new MediaGenre.MediaStyle{ Label = "Blues Rock" , Description = "As its name implies, blues rock is a combination of blues and rock music. Blues rock pieces typically have a loud beat, aggressive texture, heavy guitar sounds, and blues-scale guitar solos.\r\n\r\nBlues rock came to life in the early to mid-1960s, specifically in the United States and the United Kingdom. Bands like Led Zeppelin, ZZ Top, and The Allman Brothers Band were among the earliest bands to adopt this style of rock music." },
							new MediaGenre.MediaStyle{ Label = "Progressive Rock" , Description = "Progressive rock is an experimentation-driven subgenre of rock that emphasizes musical virtuosity, wild compositions, and conceptual lyrics. The subgenre first gained popularity back in the late 1960s with the formation of bands like Pink Floyd, Rush, and Dream Theatre.\r\n\r\nThis type of rock music is characterized by odd time signatures and long songs. Progressive rock bands also like to utilize complex compositions and instrumentations." },
							new MediaGenre.MediaStyle{ Label = "Indie Rock" , Description = "Indie rock is all about using simple instruments and a clear melody. This type of rock music emerged in the 70s-80s period in the United Kingdom and the United States. The whole point of indie rock was to combat the heavy commercialism of rock music.\r\n\r\nThe term “indie music” may refer to either independent artists or those within the indie rock genres (learn more about that in our article on What Is Indie Music). The term can be sometimes misunderstood, many artists fit into both categories. They are different but not mutually exclusive!\r\n\r\nSome of the most popular indie rock bands include Arctic Monkeys, Yeah Yeah Yeahs, Kaiser Chiefs, and The Killers." },
							new MediaGenre.MediaStyle{ Label = "Psychedelic Rock" , Description = "Psychedelic rock is heavily influenced by psychedelic culture. Bands that play this style of rock music rely on trippy studio effects, including reverb, distortion, reversed sound, and phasing to give the subgenre a distinct nature. This is usually in combination with the original use of instruments such as wah-wah pedals and electric guitars with feedback.\r\n\r\nBands like The 13th Floor Elevator, Jefferson Airplane, and The Flaming Lips are often regarded as the pioneers of psychedelic rock." },
							new MediaGenre.MediaStyle{ Label = "Acid Rock" , Description = "Acid rock was inspired by the garage punk movement in the mid-60s. Many people consider acid and psychedelic rock to be the same subgenre, while others classify psychedelic rock as a subgenre of acid rock. \r\n\r\nAcid rock incorporates long jam sessions and heavy sound distortion, accompanied by blues progressions. Jefferson Airplane, Iron Butterfly, Blue Cheer, and Pink Floyd are some of the most widely-acclaimed acid rock bands." },
							new MediaGenre.MediaStyle{ Label = "Glam Rock" , Description = "Popularized in the 1970s in the United Kingdom, glam rock was one of the most iconic cultural phenomena at that time. This style of rock music is greatly influenced by bubblegum pop. Glam rock bands usually put on wild wigs and wear unconventional costumes.\r\n\r\nThe subgenre prioritizes catchy melodies, stomping hip-shaking rhythms, and extreme theatricality. Some notable glam rock bands include Slade, T. Rex, New York Dolls, and Sweet." },
							new MediaGenre.MediaStyle{ Label = "Roots Rock" , Description = "Roots rock takes inspiration from traditional American music genres like blues, country, and folk. The earliest works of roots rock date back to the late 1960s, in response to the spread of progressive rock during that period. The subgenre was also prominent in the 1980s when heavy metal and punk rock were dominating the scene.\r\n\r\nThere are many styles of roots rock that include country rock, southern rock, blues rock, heartland rock, and swamp rock. Some of the legendary roots rock bands include Los Lobos, The Allman Brothers Band, and The Marshall Tucker Band." },
							new MediaGenre.MediaStyle{ Label = "Folk Rock" , Description = "Folk rock is basically folk music plus the heavy guitar riffs and drums of rock music. This subgenre has its roots dating back to the early to mid-1960s in the United States. \r\n\r\nBob Dylan and the Byrds are credited for being the first artists to attempt blending the 2 genres together. And even though the new hybrid folk-rock music genre didn’t appeal to the folk community back then, it went mainstream pretty quickly.\r\n\r\nThe Albion Band, The Mamas & The Papas, and “Crosby, Stills, Nash and Young” are a few of the subgenre’s well-known adopters." },
							new MediaGenre.MediaStyle{ Label = "Arena Rock" , Description = "Arena rock isn’t really considered a subgenre of rock, but more of a style of rock music that’s intended to be played in large venues. But what makes it any different from rock music as we know it? \r\n\r\nWell, for starters, arena rock places more emphasis on melody, along with the integration of loud guitar effects and anthemic choruses. Arena rock is also known for its unique visual aesthetic that usually encompasses fireworks and smoke effects.\r\n\r\nQueen is the perfect example of arena rock music. Other notable bands that popularized this style of rock music include Bon Jovi, Foreigner, and Journey." },
							new MediaGenre.MediaStyle{ Label = "Soft Rock" , Description = "The words “soft” and “rock” don’t usually go together since, well, rock music typically features hard and heavy sounds. Nevertheless, it appealed to a quite sizable audience. \r\n\r\nSoft rock appeared in the rock music scene back in the mid to late 1960s in the United States and the United Kingdom. The subgenre borrows elements from folk-rock, baroque pop, and brill building to produce a softer sound.\r\n\r\nSome notable soft rock bands encompass Air Supply, Fleetwood Mac, Hall & Oates, and Eagles." },
							new MediaGenre.MediaStyle{ Label = "Funk Rock" , Description = "Funk rock is yet another hybrid genre that combines elements from funk and rock music. The subgenre is defined by crunchy distorted guitar sounds, synths, and funky bass lines. Keyboards and drum machines are also used in fuck rock pieces.\r\n\r\nThe genre gained widespread adoption in the late 1960s and early 1970s in the United States. Albums and singles released by the Jimi Hendrix Experience, Aerosmith, and David Bowie, were the main drivers behind the funk rock movement." },
							new MediaGenre.MediaStyle{ Label = "Garage Rock" , Description = "As you might’ve already guessed, garage rock is simple rock music that’s played in a garage. The trend started in the late 1950s to early 1960s in the United States and Canada with the emergence of garage rock bands.\r\n\r\nThis subgenre is fueled by raw energy and often accompanied by easy chord progressions and untamed aggression. Bo Diddley, Little Richard, Link Wray, and Chuck Berry are some notable artists that influenced the evolution of garage rock music. \r\n\r\nLater in the mid-1960s, the debut of bands like The Beatles and The British Invasion helped the subgenre flourish." },
							new MediaGenre.MediaStyle{ Label = "Space Rock" , Description = "Space rock has a little bit of a “space geekiness” feel about it. It often comprises lengthy song structures with a focus on instrumental textures and reverb-laden guitar sounds. Lyrics that revolve around science fiction and outer space are often included, too.\r\n\r\nPink Floyd are considered the founding fathers of this interesting subgenre. Releases like Astronomy Domine, Lucifer Sam, and Interstellar Overdrive are the earliest pieces of rock music to be categorized as space rock. \r\n\r\nOther notable space rock bands include Spacemen 3, Hawkwind, and Eloy." },
							new MediaGenre.MediaStyle{ Label = "Electronic Rock" , Description = "Electronic rock is a fusion genre that combines elements of electronic and rock music. The subgenre is known for its integration of electronic instruments and beats into rock music. It often takes inspiration from other genres like hip hop, synth-pop, and techno.\r\n\r\nThis style of music got popular in the late 1960s, with characteristics like upbeat vocals, mellotrons, synths, and tape techniques. Some of the most popular electronic rock bands encompass Depeche Mode, Ratatat, Linkin Park, and Celldweller. " },
							new MediaGenre.MediaStyle{ Label = "Experimental Rock" , Description = "Experimental rock is essentially rock music with very few creative boundaries. It’s where artists test out their wildest ideas!\r\n\r\nThe subgenre first appeared in the 1960s in the United States, encompassing improvisation, odd structures and rhythms, and an avant-garde influence. Many people associate experimental rock with art rock because both subgenres heavily rely on improvisation. \r\n\r\nBands that have released pieces of experimental rock include Sonic Youth, Swans, and The Velvet Underground." },
							new MediaGenre.MediaStyle{ Label = "Surf Rock" , Description = "Surf rock was popular among the surf community in the early 1960s, specifically in southern California. Surf rock can be classified into two styles: instrumental surf and vocal surf. The former is defined by the use of reverb-heavy electric guitars to mimic the sounds of crashing waves, while the latter also adds vocal harmonies to the guitar sounds.\r\n\r\nThe Beach Boys is probably the most famous surf music band and are considered the subgenre’s founders. Other notable bands include The Surfaris, The Ventures, and The Bel-Airs." },
							new MediaGenre.MediaStyle{ Label = "Britpop" , Description = "Britpop is a style of alternative rock music that has a British influence. It emerged in the 1990s in the United Kingdom. Some say that it was more of a cultural movement that responded to US-influenced grunge music rather than a subgenre of its own. \r\n\r\nThe aim of the movement was to bring back the “British-styled” alternative rock music that was invented by The Beatles and other British rock bands. Super Grass, Oasis, and Sleeper are some of the bands that supported the movement with pieces of Britpop music." },
							new MediaGenre.MediaStyle{ Label = "Art Rock" , Description = "With an artistic vibe, the sound of art rock is super experimental, be it in the time signatures or the rhythms. This avant-garde style of rock music got notable recognition in the 1960s in the United States and the United Kingdom.\r\n\r\nPieces of art rock music are intended to be listened to while relaxing at home, instead of dancing to it. A few names that pioneered the art rock subgenre include Frank Zappa’s Mothers of Invention, The Pretty Things, Pink Floyd, and Procol Harum." },
							new MediaGenre.MediaStyle{ Label = "Stoner Rock" , Description = "Stoner rock is a fusion genre that combines not one, not two, but three genres of music into one. It borrows from subgenres such as acid rock, doom metal, and psychedelic rock. The subgenre came to life in the 1990s, and it’s characterized by groove-laden sounds, heavy bass, distortion, and a slow-mid tempo. \r\n\r\nBlack Sabbath, Kyuss, Monster Magnet, and Queen of the Stone Age are some of the most prominent names in the stoner rock music industry." },
							new MediaGenre.MediaStyle{ Label = "Instrumental Rock" , Description = "Instrumental rock is pretty much what it sounds like it is: rock music minus the lyrics. This style of music spread out in the 1950s-1960s era in the United States. \r\n\r\nIt uses the fundamental characteristics of rock music with a greater focus on catchy melodies. It’s also worth noting that, unlike lyrical rock music, instrumental rock has more solo artists. \r\n\r\nOne classic piece of instrumental rock is The Bill Doggett Combo’s “Honky Tonk”. The song featured a sinuous saxophone-organ lead and a catchy beat.\r\n\r\nSome remarkable instrumental rock artists include Steve Vai, Delicate Steve, and Ry Cooder." },
							new MediaGenre.MediaStyle{ Label = "Jazz Rock" , Description = "Jazz rock is a blend of jazz and rock genres. This style of music went mainstream in the 1960s. Jazz rock is also categorized as a subgenre of jazz fusion, a genre that combines various music genres together. \r\n\r\nJazz rock songs incorporate the essential characteristics of jazz music, accompanied by the heavy guitar riffs, bass lines, and drumming styles of rock music. The subgenre also spotlights the use of electronic instruments and dance rhythms. \r\n\r\nMahavinsu Orchestra, Blood, Sweat & Tears, and Soft Machine are among the pioneers of jazz rock." },
							new MediaGenre.MediaStyle{ Label = "Sleaze Rock" , Description = "Sleaze rock is pretty much a style of glam rock music with some elements from hard rock. Sleaze rock is often confused with glam rock, but it’s undeniable that both subgenres share lots of similarities. \r\n\r\nThe term came into existence in the 1980s. The greed decade witnessed the formation of a considerable number of sleaze rock bands, whose members wore torn jeans, mesh shirts, and outrageous wigs. \r\n\r\nSome of these bands include Hanoi Rock, L.A. Guns, and Faster Pussycat." },
							new MediaGenre.MediaStyle{ Label = "Gothic Rock" , Description = "Gothic rock emerged in the post-punk wave of the 1970s in the United Kingdom. It incorporates darker themes and sounds, reverbs, melodic bass chords, and jangly guitars. \r\n\r\nThe subgenre’s poetic nature has made it an integral part of the goth subculture. The Sisters of Mercy, Bauhaus, The Cure, and Fields of the Nephilim are a few of the greatest goth rock bands of all time. Early works of gothic rock, like Bauhaus’ Bela Lugosi’s Dead, have inspired an entire generation of goth bands. " },
							new MediaGenre.MediaStyle{ Label = "Jam Rock" , Description = "Jam rock is a relatively new style of rock music that originated in the late 1980s to early 1990s. During that period, free-form extended jams took the rock music industry by storm, signaling the birth of the jam rock subgenre. \r\n\r\nJam rock is derived from psychedelic rock music, with bands like The Allman Brothers Band, The Grateful Dead, and the Bluesy Riffs leading the scene with iconic live performances. The subgenre deeply relies on improvisation." },
							new MediaGenre.MediaStyle{ Label = "Industrial Rock" , Description = "Industrial rock is an offspring of 2 genres: industrial music and rock music. It’s defined by heavy distortion, provocative sounds, and controversial vocals and themes. The earliest works of industrial rock appeared in the late 70s, with the likes of Cromagnon, Chrome, Einstürzende Neubauten, and Throbbing Gristle contributing to the subgenre. \r\n\r\nHowever, the subgenre didn’t gain significant popularity and mainstream acceptance till the 90s. Some of the bands that helped the subgenre become mainstream include Nine Inch Nails, White Zombie, and Orgy." },
							new MediaGenre.MediaStyle{ Label = "Geek Rock" , Description = "Geek rock, or nerd rock, adds elements inspired by the geek culture to rock music. It usually includes unconventional instruments like ukuleles and accordions, along with a little bit of humor. Of course, the themes and lyrics almost always include references to the geek subculture. \r\n\r\nThe subgenre first appeared in the late 90s, making it one of the newest styles of rock music. Weezer, Nerf Herder, and Devo are some of the most popular geek rock bands." },
							new MediaGenre.MediaStyle{ Label = "Yacht Rock" , Description = "OK, this one sounds a bit eccentric, but it does exist. Yacht rock is literally a form of rock music that’s accompanied by music videos shot on yachts. The subgenre exploded in the mid-1970s to mid-1980s era. Other popular names for this style of music are west coast sound and adult-oriented rock.\r\n\r\nSo, what defines yacht rock? Well, for starters, yacht rock is melodic and strays a bit from hard rock music. It’s soft and often includes elements of R&B and jazz. Steely Dan, Toto, and The Doobie Brothers are a few names that popularized the subgenre. " },
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "Heavy Metal" , Description = "Heavy metal music is a sub-genre of rock and is characterized by loud volumes, crashing cymbals, pounding rhythms, and distorted guitars which often use drop tunings. Black Sabbath and Motorhead are two prime examples of classic heavy metal bands.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = AdaptiveKind.Audio , Label = "EDM" , Description = "EDM is short for “electronic dance music”, which is a very broad category. In a popular music context, this genre describes songs that feature classic elements from dance music, such as four-to-the-floor drum beats, synthesizers, and repeated loops.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "House" , Description = "Technically a sub-genre of EDM, house music has a huge global fanbase. Musically, it is often characterized by a tempo of between 120 to 130 BPM, with the kick drum being played on every beat." },
							new MediaGenre.MediaStyle{ Label = "Techno" , Description = "Techno shares some similarities with the house music but tends to feature electronic sounds more heavily. It is incredibly popular in the rave scene, with its powerful, thudding drum beats making the genre perfect for long dancing sessions." },
							new MediaGenre.MediaStyle{ Label = "Ambient" , Description = "With dreamy atmospheric layers that constantly change and evolve, ambient music is a unique instrumental genre. It includes a blend of acoustic and electronic instruments and samples, placing more emphasis on tonal qualities rather than rhythm." }
						}
					},
				};
				Info.Serialize();
			}
			Info._Details = new List<MediaGenre>( Deserialize() );
		}
		/// <summary>
		/// What: Adds a new Genres/styles in the singleton list
		///  Why: Enrich the singleton container with a new information 
		///  Return: the object instance of the newly updated/added data
		/// </summary>
		internal static MediaGenre Add( AdaptiveKind type , string genre , string description )
		{
			LogTrace.Label( $"{genre} , {description}" );
			lock( Info )
			{
				if( Info[genre] == null )
					Info._Details.Add( new MediaGenre { Label = genre , Description = description , Type = type } );
				return Info[genre];
			}
		}
		/// <summary>
		/// What: update the genre description
		///  Why: the end-user might want to add more details in the description or simply replace it
		/// </summary>
		/// <param name="genre"></param>
		/// <param name="description"></param>
		internal static void Update( string genre , string description )
		{
			LogTrace.Label( $"{genre} , {description}" );
			lock( Info )
			{
				if( Info[genre] != null )
					Info[genre].Description = description;
			}
		}
		/// <summary>
		/// What: remove a genre from the data set
		///  Why: the end-user want to remove a genre because never used for example.
		/// </summary>
		/// <param name="genre"></param>
		/// <param name="description"></param>
		internal static void Remove( MediaGenre mg )
		{
			LogTrace.Label( mg.Label );
			lock( Info )
			{
				Info._Details.Remove( mg );
			}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Name of the singleton
		///  Why: The name of the object is also identifying the object serialization on the support
		/// </summary>
		private const string Name = "MediaGenre";
		/// <summary>
		/// What: Filename used to load/save the content of the singleton
		///  Why: one filename access allowing a centralization of the filename management
		/// </summary>
		private static string Filename
		{
			get
			{
				FileInfo fi = new FileInfo( Path.Combine( MemoryCache.Default["DataPath"] as string , Name + ".xml" ) );
				if( !fi.Directory.Exists )
					fi.Directory.Create();
				return fi.FullName;
			}
		}
		/// <summary>
		/// What: Serialization of the singleton
		///  Why: allow to save on the file system the content of the singleton - allowing manual alteration or simple recovery by deleting the file.
		/// Note: The genre data set will be updated by the configuration user interface.
		/// </summary>
		internal void Serialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;

			LogTrace.Label( filename );

			lock( Info )
				try
				{
					if( File.Exists( filename ) )
						try
						{
							if( File.Exists( Filename + ".bak" ) )
								File.Delete( Filename + ".bak" );
							File.Copy( filename , Filename + ".bak" );
						}
						catch( Exception ) { }

					using( FileStream fs = new FileStream( filename , FileMode.Create , FileAccess.Write , FileShare.None ) )
						new XmlSerializer( typeof( MediaGenre[] ) ).Serialize( fs , Details );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , "The new media will not be saved" , $"Confirm the Data Path in the configuration file is correct and confirm read/write access to the path and the file ({filename} )" );
				}
		}
		/// <summary>
		/// What: Initialize the singleton with the data set from the hard disk
		///  Why: initialize the singleton object with the updated data from the support.
		/// </summary>
		private static MediaGenre[] Deserialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;
			try
			{
				if( File.Exists( filename ) )
				{
					LogTrace.Label( filename );
					using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
					using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
						return new XmlSerializer( typeof( MediaGenre[] ) ).Deserialize( reader ) as MediaGenre[];
				}
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
			}
			return Array.Empty<MediaGenre>();
		}
		#endregion SERIALIZATION
	}
}
