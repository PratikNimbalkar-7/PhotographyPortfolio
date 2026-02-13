pipeline {
    agent any

    tools {
        dotnet 'dotnet'   // Make sure dotnet is configured in Jenkins Tools
    }

    stages {

        // ============================
        // 1. Checkout Source Code
        // ============================
        stage('Checkout') {
            steps {
                git url: 'https://github.com/PratikNimbalkar-7/PhotographyPortfolio.git',
                    credentialsId: 'github-token'
            }
        }

        // ============================
        // 2. SonarQube Analysis
        // ============================
        stage('SonarQube Analysis') {
            steps {
                script {

                    // Get Sonar Scanner Path
                    def scannerHome = tool 'SonarScanner for MSBuild'

                    // Get Sonar Token Securely
                    withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {

                        // Connect to SonarQube Server
                        withSonarQubeEnv('SonarLocal') {

                            // Start Sonar Analysis
                            bat """
                            "${scannerHome}\\SonarScanner.MSBuild.exe" begin ^
                            /k:"PhotographyPortfolio" ^
                            /d:sonar.login=%SONAR_TOKEN%
                            """

                            // Build Project
                            bat 'dotnet restore'
                            bat 'dotnet build --configuration Release'

                            // End Sonar Analysis
                            bat """
                            "${scannerHome}\\SonarScanner.MSBuild.exe" end ^
                            /d:sonar.login=%SONAR_TOKEN%
                            """
                        }
                    }
                }
            }
        }

        // ============================
        // 3. Run Tests
        // ============================
        stage('Test') {
            steps {
                bat 'dotnet test --no-build --configuration Release'
            }
        }

        // ============================
        // 4. Deploy (Placeholder)
        // ============================
        stage('Deploy') {
            steps {
                echo 'Deploying application... (Add your deployment script here)'
            }
        }
    }

    post {
        success {
            echo '✅ Pipeline completed successfully!'
        }

        failure {
            echo '❌ Pipeline failed. Check logs.'
        }
    }
}
