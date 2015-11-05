/*
* This is a re-implementation of naive text summarization algorithm
* Originally created by Shlomi Babluki
* Translated to C# by Anatoly Ivanov
*/

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace SummaryTool {
	public class NaiveSummation {
		// Naive method for splitting a text into sentences
    	public static string[] SplitContentToSentences(string content) {
			return content.Split(new [] { "\n", ". "}, StringSplitOptions.RemoveEmptyEntries);
		}
		// Naive method for splitting a text into paragraphs
    	public static string[] SplitContentToParagraphs(string content) {
			return content.Split(new [] { "\n\n", "\n\t\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

    	// Caculate the intersection between 2 sentences
    	public static int SentenceIntersection(string sentence1, string sentence2) {
			// Naively split the sentences into words/tokens
			var set1 = new HashSet<string>(sentence1.Split(' '));
			var set2 = new HashSet<string>(sentence2.Split(' '));
			
			// If average set z=size is zero, return zero immediately
        	// That way we speed the analysis up and avoid division by zero in the next statement
			int averageSetSize = (set1.Count + set2.Count) / 2;
			if (averageSetSize==0)
            	return 0;

			// we intersect the sets and keep the result in set1
        	set1.IntersectWith(set2);
			
			// And then normalize the result by the average number of words
        	return set1.Count * 100 / averageSetSize;	
		}
		
		// Format a sentence - remove all non-alphbetic chars from the sentence
    	// We'll use the formatted sentence as a key in our sentences dictionary
    	private static readonly Regex whitespaces = new Regex(@"\W+", RegexOptions.Compiled);
		public static string FormatSentence(string sentence) {
        	return whitespaces.Replace(sentence,"");
		}
		
		// Convert the content into a dictionary <K, V>
    	// k = The formatted sentence
    	// V = The rank of the sentence
    	public static Dictionary<string, int> GetSentenceRanked(string content) {
	        // Split the content into sentences
        	var sentences = SplitContentToSentences(content);

        	// Calculate the intersection of every two sentences
			// i.e. rank the sentences against each other
        	int n = sentences.Length;
			var values = new int [n,n];
			
			for (int i =0; i<n; i++) {
				for (int j = 0; j<n; j++) {
					if (i==j) continue;
					values[i,j] = SentenceIntersection(sentences[i], sentences[j]);
				}
			}
			
        	// Build the sentences dictionary
        	// The score of a sentences is the sum of all its intersection
        	var sentences_dic = new Dictionary<string, int>();
        	for (int i = 0; i<n; i++) {
				int score = 0;
				for (int j = 0; j<n; j++) {
					if (i==j) continue;
					score += values[i,j];
				}
				
				sentences_dic[FormatSentence(sentences[i])] = score;
			}
			return sentences_dic;
		}		
		
		// Find the best sentence in a paragraph
		public static string GetBestSentence(string paragraph, Dictionary<string, int> sentences_dic) {
   			// Split the paragraph into sentences
        	var sentences = SplitContentToSentences(paragraph);

			//Ignore short paragraphs
			if (sentences.Length < 2)
				return null;

        	// Get the best sentence according to the sentences dictionary
        	string best_sentence = null;
        	int max_value = 0;
        	foreach (var s in sentences) {
            	var strip_s = FormatSentence(s);
				if (!string.IsNullOrWhiteSpace(strip_s) && sentences_dic[strip_s] > max_value) {
						max_value = sentences_dic[strip_s];
						best_sentence = s;
				}
			}
			return best_sentence;
		}
		
		// Build the summary
		public static string GetSummary(string title, string content, Dictionary<string, int> sentences_dic) {
        	// Split the content into paragraphs
        	var paragraphs = SplitContentToParagraphs(content);

			// Add the title
			var summary = new StringBuilder();
			summary.AppendLine(title.Trim());
			summary.AppendLine("");

        	// Add the best sentence from each paragraph
        	foreach (var paragraph in paragraphs) {
            	var sentence = GetBestSentence(paragraph, sentences_dic);
            	if (!string.IsNullOrWhiteSpace(sentence)) {
                	summary.AppendLine(sentence.Trim());
				}
			}

        	return summary.ToString();
		}
	}
	public class Program {
		public static void Main(string[] args){
			
			var title = @"Swayy is a beautiful new dashboard for discovering and curating online content [Invites]";

    		var content = @"
    Lior Degani, the Co-Founder and head of Marketing of Swayy, pinged me last week when I was in California to tell me about his startup and give me beta access. I heard his pitch and was skeptical. I was also tired, cranky and missing my kids – so my frame of mind wasn’t the most positive.
    I went into Swayy to check it out, and when it asked for access to my Twitter and permission to tweet from my account, all I could think was, “If this thing spams my Twitter account I am going to bitch-slap him all over the Internet.” Fortunately that thought stayed in my head, and not out of my mouth.
	One week later, I’m totally addicted to Swayy and glad I said nothing about the spam (it doesn’t send out spam tweets but I liked the line too much to not use it for this article). I pinged Lior on Facebook with a request for a beta access code for TNW readers. I also asked how soon can I write about it. It’s that good. Seriously. I use every content curation service online. It really is That Good.


	What is Swayy? 
	It’s like Percolate and LinkedIn recommended articles, mixed with trending keywords for the topics you find interesting, combined with an analytics dashboard that shows the trends of what you do and how people react to it. I like it for the simplicity and accuracy of the content curation. Everything I’m actually interested in reading is in one place – I don’t have to skip from another major tech blog over to Harvard Business Review then hop over to another major tech or business blog. It’s all in there. And it has saved me So Much Time
    After I decided that I trusted the service, I added my Facebook and LinkedIn accounts. The content just got That Much Better. I can share from the service itself, but I generally prefer reading the actual post first – so I end up sharing it from the main link, using Swayy more as a service for discovery.
    I’m also finding myself checking out trending keywords more often (more often than never, which is how often I do it on Twitter.com).
    The analytics side isn’t as interesting for me right now, but that could be due to the fact that I’ve barely been online since I came back from the US last weekend. The graphs also haven’t given me any particularly special insights as I can’t see which post got the actual feedback on the graph side (however there are numbers on the Timeline side.) This is a Beta though, and new features are being added and improved daily. I’m sure this is on the list. As they say, if you aren’t launching with something you’re embarrassed by, you’ve waited too long to launch.
    It was the suggested content that impressed me the most. The articles really are spot on – which is why I pinged Lior again to ask a few questions
	
	
	How do you choose the articles listed on the site? Is there an algorithm involved? And is there any IP?
    Yes, we’re in the process of filing a patent for it. But basically the system works with a Natural Language Processing Engine. Actually, there are several parts for the content matching, but besides analyzing what topics the articles are talking about, we have machine learning algorithms that match you to the relevant suggested stuff. For example, if you shared an article about Zuck that got a good reaction from your followers, we might offer you another one about Kevin Systrom (just a simple example).


	Who came up with the idea for Swayy, and why? And what’s your business model?
	Our business model is a subscription model for extra social accounts (extra Facebook / Twitter, etc) and team collaboration.
    The idea was born from our day-to-day need to be active on social media, look for the best content to share with our followers, grow them, and measure what content works best.


	Who is on the team?
    Ohad Frankfurt is the CEO, Shlomi Babluki is the CTO and Oz Katz does Product and Engineering, and I [Lior Degani] do Marketing. The four of us are the founders. Oz and I were in 8200 [an elite Israeli army unit] together. Emily Engelson does Community Management and Graphic Design.
    If you use Percolate or read LinkedIn’s recommended posts I think you’ll love Swayy.

    ➤ Want to try Swayy out without having to wait? Go to this secret URL and enter the promotion code thenextweb . The first 300 people to use the code will get access.
    Image credit: Thinkstock
    ";

    		
    	// Build the sentences dictionary
    	var sentences_dic = NaiveSummation.GetSentenceRanked(content);
		foreach (var s in sentences_dic.Keys) {
			Console.WriteLine("{0}->{1}", s, sentences_dic[s]);
		}

    	// Build the summary with the sentences dictionary
    	var summary = NaiveSummation.GetSummary(title, content, sentences_dic);

    	// Print the summary
    	Console.WriteLine(summary);

    	// Print the ratio between the summary length and the original length
    	Console.WriteLine("\nOriginal Length: {0}\nSummary Length: {1}\nSummary Ratio: {2}", 
			title.Length + content.Length,
			summary.Length,
			100 - (100 * summary.Length/(title.Length + content.Length))
			);
			
		}
	}
}
