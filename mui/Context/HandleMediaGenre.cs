using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	internal sealed class HandleMediaGenre
	{
		#region LOCAL VARIABLE
		/// <summary>The details of the security list for the streaming</summary>
		private List<MediaGenre> _Details = new List<MediaGenre>();
		#endregion LOCAL VARIABLE

		#region ACCESSORS
		internal MediaGenre[] Details => _Details.ToArray();
		/// <summary>Gets the <see cref="RateSecurityData"/> with the specified currency.</summary>
		/// <value>The <see cref="RateSecurityData"/>.</value>
		/// <param name="currency">The currency.</param>
		/// <returns></returns>
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
		/// <summary>Gets the information.</summary>
		/// <value>The information.</value>
		internal static HandleMediaGenre Info { get; private set; } = new HandleMediaGenre();
		#endregion SINGLETON

		#region PUBLIC METHODS
		public static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
			{
				LogTrace.Label();
				Info._Details = new List<MediaGenre>()
				{
					//	https://www.studiobinder.com/blog/movie-genres-list/#animation-genre
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Video , Label = "Action" , Description = "Movies in the action genre are defined by risk and stakes.\nWhile many movies may feature an action sequence, to be appropriately categorized inside the action genre, the bulk of the content must be action-oriented, including fight scenes, stunts, car chases, and general danger.",
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
						Type = MediaGenre.MediaType.Video , Label = "Animation" , Description = "The animation genre is defined by inanimate objects being manipulated to appear as though they are living.",
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
						Type = MediaGenre.MediaType.Video , Label = "Comedy" , Description = "The comedy genre is defined by events that are intended to make someone laugh, no matter if the story is macabre, droll, or zany. Comedy can be found in most movies, but if the majority of the film is intended to be a comedy you may safely place it in this genre.",
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
						Type = MediaGenre.MediaType.Video , Label = "Cooking" , Description = "All the cooking video",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Every day" , Description = "Every day cooking recipes." },
							new MediaGenre.MediaStyle{ Label = "Connoisseur" , Description = "Top end culinary recipes." },
							new MediaGenre.MediaStyle{ Label = "Tips" , Description = "cooking tips and recipes for remedy, conserve, ..." }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Video , Label = "Crime" , Description = "The crime genre deals with both sides of the criminal justice system but does not focus on legislative matters or civil suits and legal actions. The best crime movies often occupy moral gray areas where heroes and villains are much harder to define. ",
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
						Type = MediaGenre.MediaType.Video , Label = "Documentaries" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Video , Label = "Scientific" , Description = "All lessons or courses on-line",
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
						Type = MediaGenre.MediaType.Video , Label = "Drama" , Description = "The drama genre is defined by conflict and often looks to reality rather than sensationalism. Emotions and intense situations are the focus, but where other genres might use unique or exciting moments to create a feeling, movies in the drama genre focus on common occurrences.",
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
						Type = MediaGenre.MediaType.Video , Label = "Experiemental" , Description = "The experimental genre is often defined by the idea that the work of art and entertainment does not fit into a particular genre or sub-genre, and is intended as such. Experimental art can completely forego a cohesive narrative in exchange for an emotional response or nothing at all. ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Surrealist" , Description = "Surrealism cannot be stylistically defined, and this is the point of the sub-genre itself. The intention of surrealist art is to act as an activity to broaden horizons, either of oneself or of others. Surrealist art often uses irrational imagery to activate the subconscious mind." },
							new MediaGenre.MediaStyle{ Label = "Absurdist" , Description = "The absurdist sub-genre focuses on characters who experience situations that suggest there is no central purpose to life. Another way to frame it is a set of experiences that catalyze a descent into nihilism." }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Video , Label = "Fantasy" , Description = "The fantasy genre is defined by both circumstance and setting inside a fictional universe with an unrealistic set of natural laws. The possibilities of fantasy are nearly endless, but the movies will often be inspired by or incorporate human myths.",
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
						Type = MediaGenre.MediaType.Video , Label = "Historical" , Description = "The historical genre can be split into two sections. One deals with accurate representations of historical accounts which can include biographies, autobiographies, and memoirs. The other section is made up of fictional movies that are placed inside an accurate depiction of a historical setting.\nThe accuracy of a historical story is measured against historical accounts, not fact, as there can never be a perfectly factual account of any event without first-hand experience. ",
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
						Type = MediaGenre.MediaType.Video , Label = "Horror" , Description = "The horror genre is centered upon depicting terrifying or macabre events for the sake of entertainment. A thriller might tease the possibility of a terrible event, whereas a horror film will deliver all throughout the film. The best horror movies are designed to get the heart pumping and to show us a glimpse of the unknown. ",
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
						Type = MediaGenre.MediaType.Video , Label = "Romance" , Description = "The romance genre is defined by intimate relationships. Sometimes these movies can have a darker twist, but the idea is to lean on the natural conflict derived from the pursuit of intimacy and love.  ",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Drama" , Description = "The romance-drama sub-genre is defined by the conflict generated from a  romantic relationship. What makes a romance-drama different from a romantic-thriller is both the source of the drama but also the intentions and motivations that drive each character’s perspective." },
							new MediaGenre.MediaStyle{ Label = "Thriller" , Description = "The romance-thriller sub-genre is defined by a suspenseful story that includes and is most likely based around a romantic relationship. Some romantic thrillers can divert into psychological thrillers where the relationship is used to manipulate, but most focus on the characters attempting to make it out of events so that they may be together." },
							new MediaGenre.MediaStyle{ Label = "Period " , Description = "A period-romance story is defined by the setting and can include and incorporate other romance sub-genres. The setting must be a historical time period, and often will adhere to the societal norms of the specific time period, though some movies have taken a more revisionist approach." }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Video , Label = "Science Fiction" , Description = "Science fiction movies are defined by a mixture of speculation and science. While fantasy will explain through or make use of magic and mysticism, science fiction will use the changes and trajectory of technology and science. Science fiction will often incorporate space, biology, energy, time, and any other observable science.",
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
						Type = MediaGenre.MediaType.Video , Label = "Thriller" , Description = "A thriller story is mostly about the emotional purpose, which is to elicit strong emotions, mostly dealing with generating suspense and anxiety. No matter what the specific plot, the best thrillers get your heart racing.",
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
						Type = MediaGenre.MediaType.Video , Label = "Western" , Description = "Westerns are defined by their setting and time period. The story needs to take place in the American West, which begins as far east as Missouri and extends to the Pacific ocean. They’re set during the 19th century, and will often feature horse riding, military expansion, violent and non-violent interaction with Native American tribes, the creation of railways, gunfights, and technology created during the industrial revolution. ",
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
						Type = MediaGenre.MediaType.Video , Label = "Others" , Description = "",
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
						Type = MediaGenre.MediaType.Audio , Label = "Classical" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Avant-Garde" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Baroque" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Concerto" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Renaissance" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Romantic " , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Symphony" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Chamber Music" , Description = "" }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Avant-garde & Experimental" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Instrumental" , Description = "" },
							new MediaGenre.MediaStyle{ Label = "Psychedelic music" , Description = "" }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Blues" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Country" , Description = "Country music can be traced back to the beginning of the twentieth century. It was created mainly in the south of the USA, by working-class people. These people would use country as a means to tell stories through music, about the realities of everyday life and their perspectives.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Easy Listening" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Electronic" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Folk" , Description = "Folk music has existed in many different parts of the world for centuries. Traditionally, this genre is essentially built upon people gathering to sing and play songs with others in their community.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Hip Hop" , Description = "In its relatively short history, hip-hop has emerged as one of the most popular and innovative genres of music. Hip-hop originated in the Bronx, a borough of New York City, in the late 1970s when DJs would use samples and breakbeats to create backing tracks for MCs to rap over.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Jazz" , Description = "In the early 20th century, musicians in the city of New Orleans experimented by blending musical elements from European and African genres. This resulted in the origination of jazz, which would go on to become one of the most popular and unique musical styles in existence.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Pop" , Description = "Pop music is perhaps the most obvious addition to this list. It could be said that pop is a compilation of other genres, which are considered to be mainstream in a certain time period.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Funk" , Description = "Like the soul, funk was also the result of African Americans blending jazz and R&B. Funk has a strong rhythmic pulse, prominent bass lines, and syncopated rhythm guitar playing." },
							new MediaGenre.MediaStyle{ Label = "Reggae" , Description = "Reggae was invented in Jamaica in the late 60s, and quickly become the country’s favorite music genre. In the decades that followed, it reached the UK, USA, and Africa, where it amassed huge audiences." },
							new MediaGenre.MediaStyle{ Label = "Disco" , Description = "Disco music rose to prominence in the late 60s and early 70s, making its way into the upmarket nightclubs of major US cities." },
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "R&B & Soul" , Description = "R&B, or rhythm and blues, is rooted in African-American culture from the 1940s. In the decades that followed, record labels used the term to describe recordings that were targeted towards that community in the US, and the style eventually inspired many rock artists of the 1960s.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Rock" , Description = "One of the top musical genres in existence, rock music originated in the 1940s and 50s in the form of rock & roll. However, its roots can be traced back to the rhythm and blues of the African-American culture in the 1920s, merged with country music.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Punk Rock" , Description = "Punk rock has an aggressive sound, with fast-paced tempos and simple guitar riffs often played using only downstrokes. It was seen as a departure from the technical styles of the main music genres that had dominated the 1970s, with bands like The Ramones, The Sex Pistols, and The Clash bringing it into the public eye." },
							new MediaGenre.MediaStyle{ Label = "Grunge" , Description = "In the 1980s in Seattle, a collective of aspiring musicians had become disillusioned with the mainstream rock music that dominated the radio stations. Out of their frustration, a new genre was born – grunge.As the genre amassed a growing fan base, bands like Nirvana and Pearl Jam were propelled to stardom. Highly distorted guitar riffs, pounding drums and gritty vocals are three common characteristics of this genre. It continues to inspire the new generation of rock musicians today." }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Heavy Metal" , Description = "Heavy metal music is a sub-genre of rock and is characterized by loud volumes, crashing cymbals, pounding rhythms, and distorted guitars which often use drop tunings. Black Sabbath and Motorhead are two prime examples of classic heavy metal bands.",
						Details = new MediaGenre.MediaStyle[]
						{
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "EDM" , Description = "EDM is short for “electronic dance music”, which is a very broad category. In a popular music context, this genre describes songs that feature classic elements from dance music, such as four-to-the-floor drum beats, synthesizers, and repeated loops.",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "House" , Description = "Technically a sub-genre of EDM, house music has a huge global fanbase. Musically, it is often characterized by a tempo of between 120 to 130 BPM, with the kick drum being played on every beat." },
							new MediaGenre.MediaStyle{ Label = "Techno" , Description = "Techno shares some similarities with the house music but tends to feature electronic sounds more heavily. It is incredibly popular in the rave scene, with its powerful, thudding drum beats making the genre perfect for long dancing sessions." },
							new MediaGenre.MediaStyle{ Label = "Ambient" , Description = "With dreamy atmospheric layers that constantly change and evolve, ambient music is a unique instrumental genre. It includes a blend of acoustic and electronic instruments and samples, placing more emphasis on tonal qualities rather than rhythm." }
						}
					},
					new MediaGenre()
					{
						Type = MediaGenre.MediaType.Audio , Label = "Punk" , Description = "",
						Details = new MediaGenre.MediaStyle[]
						{
							new MediaGenre.MediaStyle{ Label = "Indie Rock" , Description = "In the past thirty years or so, indie has developed into one of the most popular sub-genres of rock music. With its D.I.Y ethos inspired largely by punk, indie reached its peak in popularity in the 2000s, with bands like The Strokes and Arctic Monkeys paving the way." },
							new MediaGenre.MediaStyle{ Label = "" , Description = "" }
						}
					}
				};
				Info.Serialize();
			}
			Info._Details = new List<MediaGenre>( Deserialize() );
		}
		public static MediaGenre Add( string genre , string description )
		{
			LogTrace.Label( $"{genre} , {description}" );
			lock( Info )
			{
				if( Info[genre] == null )
					Info._Details.Add( new MediaGenre { Label = genre , Description = description } );
				return Info[genre];
			}
		}
		public static void Update( string genre , string description )
		{
			LogTrace.Label( $"{genre} , {description}" );
			lock( Info )
			{
				if( Info[genre] != null )
					Info[genre].Description = description;
			}
		}
		public static void Remove( MediaGenre mg )
		{
			LogTrace.Label( mg.Label );
			lock( Info )
			{
				Info._Details.Remove( mg );
			}
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>The name</summary>
		internal const string Name = "MediaGenre";

		/// <summary>Gets the filename.</summary>
		/// <value>The filename associated with the object.</value>
		internal static string Filename
		{
			get
			{
				FileInfo fi = new FileInfo( Path.Combine( MemoryCache.Default["DataPath"] as string , Name + ".xml" ) );
				if( !fi.Directory.Exists )
					fi.Directory.Create();
				return fi.FullName;
			}
		}
		/// <summary>Serializes the specified filename.</summary>
		/// <param name="filename">The filename.</param>
		internal void Serialize( string filename = null )
		{
			if( filename == null )
				filename = Filename;

			LogTrace.Label( filename );

			lock( Info )
				try
				{
					if( File.Exists( filename ) )
						try
						{
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
		/// <summary>Deserializes the specified filename <param name="filename">The filename.</param></summary>
		/// m&gt;
		/// <returns></returns>
		private static MediaGenre[] Deserialize( string filename = null )
		{
			if( filename == null )
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
